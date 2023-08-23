using TradeLogic.APIModels.Quotes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using AutoTradeMobile.DataClasses;
using System.Globalization;
using TradeLogic;
using TradeLogic.APIModels.Accounts.portfolio;

namespace AutoTradeMobile
{
    public partial class SymbolData : ObservableObject
    {
        const string StudiesFileName = "Studies.txt";
        static Object _studiesLoadLock = new Object();

        public SymbolData()
        {
            lock (_studiesLoadLock)
            {
                //load up the saved studies
                Trace.WriteLine("Loading Studies from file");
                Studies.LoadFromFile(StudiesFileName);
                Trace.WriteLine($"{Studies.Count} Studies");
                if (Studies.Count == 0)
                {
                    Trace.WriteLine("Adding default Study Config");
                    Studies.Add(new StudyConfig()
                    {
                        Period = 5,
                        UptrendAmountRequired = 0.10
                    });
                    Studies.Add(new StudyConfig()
                    {
                        Period = 10,
                        UptrendAmountRequired = 0.01
                    });
                    Studies.PersistToFile(StudiesFileName);
                }
            }

        }

        [ObservableProperty]
        ObservableCollection<Tick> ticks = new();

        [ObservableProperty]
        ObservableCollection<Minute> minutes = new();

        [ObservableProperty]
        public ObservableCollection<StudyConfig> studies = new();

        [ObservableProperty]
        CurrentPosition currentPosition = new();

        [ObservableProperty]
        private string _Symbol = "No Data Received Yet";

        [ObservableProperty]
        int tickCount;

        [ObservableProperty]
        long lastTime;

        [ObservableProperty]
        bool isAfterHours;

        [ObservableProperty]
        string quoteStatus;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ChangeCloseColor))]
        double changeClose;

        [ObservableProperty]
        double changeClosePercentage;

        public Color ChangeCloseColor
        {
            get
            {
                return ChangeClose >= 0 ? Colors.Green : Colors.Red;
            }
        }


        [ObservableProperty]
        long totalVolume;

        [ObservableProperty]
        double lastTrade;

        [ObservableProperty]
        double bid;

        [ObservableProperty]
        double ask;

        [ObservableProperty]
        Minute lastMinute;

        [ObservableProperty]
        double todayHigh;

        [ObservableProperty]
        double todayLow;

        public void addQuote(GetQuotesResponse quote)
        {
            if (Ticks.Count == 0)
            {
                Symbol = quote.QuoteData.Product.Symbol.ToUpper();
            }
            IsAfterHours = quote.QuoteData.AhFlag;
            QuoteStatus = quote.QuoteData.QuoteStatus;
            LastTime = quote.QuoteData.DateTimeUTC;

            if (quote.QuoteData.All == null && quote.QuoteData.Intraday == null)
            {
                throw new InvalidOperationException("Cannor process quote, missing both quote.QuoteData.All and quote.QuoteData.Intraday");
            }

            Tick t = new();

            if (quote.QuoteData.All != null)
            {
                //not supported yet
                throw new NotImplementedException("not supporting All quote type yet");
            }
            else if (quote.QuoteData.Intraday != null)
            {
                //global properties
                ChangeClose = quote.QuoteData.Intraday.ChangeClose;
                ChangeClosePercentage = quote.QuoteData.Intraday.ChangeClosePercentage;
                Bid = quote.QuoteData.Intraday.Bid;
                Ask = quote.QuoteData.Intraday.Ask;
                TotalVolume = quote.QuoteData.Intraday.TotalVolume;
                LastTrade = quote.QuoteData.Intraday.LastTrade;
                TodayLow = quote.QuoteData.Intraday.Low;
                TodayHigh = quote.QuoteData.Intraday.High;

                //tick properties
                string format = "HH:mm:ss EDT MM-dd-yyyy";
                DateTime dateTime;
                if (DateTime.TryParseExact(quote.QuoteData.DateTime, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime) == false)
                {
                    if (DateTime.TryParse(quote.QuoteData.DateTime, out dateTime) == false)
                    {
                        dateTime = DateTime.Now;
                    }
                }

                t.Time = dateTime;
                t.Ask = quote.QuoteData.Intraday.Ask;
                t.Bid = quote.QuoteData.Intraday.Bid;
                t.LastTrade = quote.QuoteData.Intraday.LastTrade;
            }

            Ticks.Add(t);
            TickCount++;

            //aggrigate the tick into the minutes
            if (LastMinute == null || LastMinute.TradeMinute != t.MinuteTime)
            {
                LastMinute = t.ToMinute(LastMinute);
                AddToMinutes(LastMinute);
            }

            LastMinute.AddTick(t);
            ProcessStudies();
            EvalForTrade();
            RecalculateCurrentPosition(t);
            RefreshLastMinute();

        }

        private void RecalculateCurrentPosition(Tick t)
        {
            CurrentPosition?.UpdateMarketValue(t.LastTrade);
        }

        private void EvalForTrade()
        {
            //if there is an order pending then exit
            if (TradeApp.IsOrderPending()) { return; }
            if (LastMinute == null) { return; }//first minute nothing to do

            bool CanBuy;
            bool CanSell;
            int? MaxBuy = null;
            //do we have shares in play?
            if ((CurrentPosition?.Quantity ?? default(double)) == 0)
            {
                CanBuy = true;
                CanSell = false;
            }
            else if (CurrentPosition?.Quantity < LastMinute.FirstStudy.MaxSharesInPlay)
            {
                //no we can buy if needed
                CanBuy = true;
                CanSell = true;
                MaxBuy = (int)(LastMinute.FirstStudy.MaxSharesInPlay - CurrentPosition?.Quantity ?? default(double));
            }
            else
            {
                //only sell is possible
                CanBuy = false;
                CanSell = true;
            }

            ProcessOrderLogic(CanBuy, CanSell, MaxBuy);
        }

        private void ProcessOrderLogic(bool CanBuy, bool CanSell, int? MaxBuy)
        {
            //Trace.WriteLine($"CanBuy {CanBuy} MinuteChange : {LastMinute.MinuteChange.ToString("c")}, FirstStudyChange {LastMinute.FirstStudyChange.ToString("c")}, SecondStudyChange {LastMinute.SecondStudyChange.ToString("c")}");

            if (CanBuy && LastMinute.MinuteChange > 0 && LastMinute.FirstStudyChange > LastMinute.FirstStudy.UptrendAmountRequired && LastMinute.SecondStudyChange > 0 && Minutes.Count > LastMinute.SecondStudy.Period)
            {

                //buy order
                int MaxOrderSize = MaxBuy ?? LastMinute.FirstStudy.DefaultOrderSize;
                var orderRequest = new TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody(
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderTypes.EQ,
                    LastMinute.OrderKey,
                    Symbol,
                    MaxOrderSize,
                    LastMinute.Ticks.Last().Ask,
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderAction.BUY
                    );
                TradeApp.AddOrderToQueue(orderRequest);
            }
            else if (CanSell && (LastMinute.FirstStudyChange < 0))
            {
                Trace.WriteLineIf(LastMinute.FirstStudyChange < 0, $"Sell : LastMinute.FirstStudyChange:{LastMinute.FirstStudyChange} < 0");

                //sell order
                var orderRequest = new TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody(
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderTypes.EQ,
                    LastMinute.OrderKey,
                    Symbol,
                    LastMinute.FirstStudy.MaxSharesInPlay,
                    LastMinute.Ticks.Last().Bid,
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderAction.SELL
                    );
                TradeApp.AddOrderToQueue(orderRequest);

            }
        }

        private void ProcessStudies()
        {
            int studyIndex = 0;
            foreach (var currentStudy in Studies)
            {
                if (currentStudy != null)
                {
                    var StudyData = Minutes.OrderByDescending(m => m.LastTickTime).Take(currentStudy.Period);
                    double StudyValue = 0;
                    switch (currentStudy.Field)
                    {
                        case StudyConfig.FieldName.open:
                            StudyValue = StudyData.Select(sd => sd.Open).DefaultIfEmpty(LastMinute.AverageTrade).Average();
                            break;
                        case StudyConfig.FieldName.high:
                            StudyValue = StudyData.Select(sd => sd.High).DefaultIfEmpty(LastMinute.AverageTrade).Average();
                            break;
                        case StudyConfig.FieldName.low:
                            StudyValue = StudyData.Select(sd => sd.Low).DefaultIfEmpty(LastMinute.AverageTrade).Average();
                            break;
                        case StudyConfig.FieldName.close:
                            StudyValue = StudyData.Select(sd => sd.Close).DefaultIfEmpty(LastMinute.AverageTrade).Average();
                            break;
                        default:
                            break;
                    }
                    switch (studyIndex)
                    {
                        case 0:
                            LastMinute.FirstStudy = currentStudy;
                            LastMinute.FirstStudyValue = StudyValue;
                            break;

                        case 1:
                            LastMinute.SecondStudy = currentStudy;
                            LastMinute.SecondStudyValue = StudyValue;
                            break;
                    }
                }
                studyIndex++;
            }
        }

        private void RefreshLastMinute()
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(
                        () =>
                        Minutes[Minutes.Count - 1] = LastMinute
                        )
                    );
            }
            else
            {
                Minutes[Minutes.Count - 1] = LastMinute;
            }

        }

        private void AddToMinutes(Minute thisMinute)
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(() => Minutes.Add(thisMinute)));
            }
            else
            {
                Minutes.Add(thisMinute);
            }
            Trace.WriteLine($"New Minute {thisMinute.TradeMinute}");
        }

        private int portfolioResponseCount = 0;
        internal void ProcessPortfolioResponseData(List<Position> positions)
        {
            portfolioResponseCount++;
            var position = positions
                .Where(p => p.Product.SecurityType.Equals("EQ"))
                .GroupBy(p => p.Product.Symbol)
                .Select(group => new CurrentPosition(portfolioResponseCount, group)).FirstOrDefault();

            CurrentPosition = position;
        }


    }

}