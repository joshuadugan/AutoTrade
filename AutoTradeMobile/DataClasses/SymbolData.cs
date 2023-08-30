using TradeLogic.APIModels.Quotes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using AutoTradeMobile.DataClasses;
using System.Globalization;
using TradeLogic;
using TradeLogic.APIModels.Accounts.portfolio;
using Skender.Stock.Indicators;
using System.ComponentModel;
using CommunityToolkit.Maui.Core.Extensions;

namespace AutoTradeMobile
{
    public partial class SymbolData : ObservableObject
    {
        const string StudiesFileName = "Studies.txt";

        public SymbolData()
        {
            Task.Run(() => LoadStudies());
        }

        private void LoadStudies()
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
                    UptrendAmountRequired = 0.20m,
                    Type = StudyConfig.StudyType.ALMA
                });
                Studies.Add(new StudyConfig()
                {
                    Period = 15,
                    UptrendAmountRequired = 0.01m,
                    Type = StudyConfig.StudyType.ALMA
                });
                Studies.PersistToFile(StudiesFileName);
            }

            FirstStudy = Studies[0];
            SecondStudy = Studies[1];
        }

        [ObservableProperty]
        StudyConfig firstStudy;

        [ObservableProperty]
        StudyConfig secondStudy;

        [ObservableProperty]
        ObservableCollection<Tick> ticks = new();

        [ObservableProperty]
        ObservableCollection<Minute> chartMinutes = new();

        public List<Minute> AllMinutes { get; private set; } = new();

        public string TradingDuration
        {
            get
            {
                return new TimeSpan(0, AllMinutes.Count, 0).ToString("%h'h '%m'm'");
            }
        }

        [ObservableProperty]
        public ObservableCollection<StudyConfig> studies = new();

        [ObservableProperty]
        decimal velocityTradeOrderValue = .50m;

        [ObservableProperty]
        decimal velocityTradeTrailingStopValue = .50m;

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
        decimal changeClose;

        [ObservableProperty]
        decimal changeClosePercentage;

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
        decimal lastTrade;

        [ObservableProperty]
        decimal bid;

        [ObservableProperty]
        decimal ask;

        [ObservableProperty]
        Minute lastMinute;

        [ObservableProperty]
        decimal todayHigh;

        [ObservableProperty]
        decimal todayLow;

        [ObservableProperty]
        decimal sixtyTicksChange;

        [ObservableProperty]
        decimal sixtyTicksAverage;

        public Color MinuteAverageChangeColor
        {
            get
            {
                return MinuteAverageChange >= 0 ? Colors.Green : Colors.Red;
            }
        }

        public decimal TotalVelocity
        {
            get
            {
                return MinuteAverageChange + (LastMinute?.FirstStudyChange ?? 0) + (LastMinute?.SecondStudyChange ?? 0);
            }
        }

        public Color TotalVelocityColor
        {
            get
            {
                return TotalVelocity >= 0 ? Colors.Green : Colors.Red;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MinuteAverageChangeColor))]
        [NotifyPropertyChangedFor(nameof(TotalVelocity))]
        [NotifyPropertyChangedFor(nameof(TotalVelocityColor))]
        decimal minuteAverageChange;

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
                throw new InvalidOperationException("Cannot process quote, missing both quote.QuoteData.All and quote.QuoteData.Intraday");
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
                t.Volume = quote.QuoteData.Intraday.TotalVolume;
            }

            Ticks.Add(t);
            TickCount++;

            var sixtyTicks = Ticks.TakeLast(60);
            SixtyTicksAverage = sixtyTicks.Average(st => st.LastTrade);
            SixtyTicksChange = sixtyTicks.Last().LastTrade - sixtyTicks.First().LastTrade;

            //aggregate the tick into the minutes
            if (LastMinute == null || LastMinute.TradeMinute != t.MinuteTime)
            {
                LastMinute = t.ToMinute(LastMinute);
                AllMinutes.Add(LastMinute);
                OnPropertyChanged(nameof(TradingDuration));
            }
            LastMinute.AddTick(t);

            ProcessStudies();
            EvalForTrade();
            RecalculateCurrentPosition(t);
            ProcessChartMinutes();

        }

        private void ProcessStudies()
        {
            int studyIndex = 0;
            foreach (var currentStudy in Studies)
            {
                var quotes = AllMinutes;
                var lookbackPeriods = currentStudy.Period;
                var lastMinuteAverage = quotes.Last().AverageTrade;
                var previousMinuteAverage = quotes.SkipLast(1).LastOrDefault()?.AverageTrade ?? lastMinuteAverage;
                MinuteAverageChange = lastMinuteAverage - previousMinuteAverage;

                decimal StudyValue = LastMinute.Close;
                switch (currentStudy.Type)
                {
                    case StudyConfig.StudyType.SMA:
                        StudyValue = quotes.GetSma(lookbackPeriods).LastOrDefault()?.Sma.ToDecimal() ?? LastMinute.Close;
                        break;
                    case StudyConfig.StudyType.EMA:
                        StudyValue = quotes.GetEma(lookbackPeriods).LastOrDefault()?.Ema.ToDecimal() ?? LastMinute.Close;
                        break;
                    case StudyConfig.StudyType.VWMA:
                        StudyValue = quotes.GetVwma(lookbackPeriods).LastOrDefault()?.Vwma.ToDecimal() ?? LastMinute.Close;
                        break;
                    case StudyConfig.StudyType.ALMA:
                        StudyValue = quotes.GetAlma(lookbackPeriods, offset: 0.85, sigma: 6).LastOrDefault()?.Alma.ToDecimal() ?? LastMinute.Close;
                        break;
                    default:
                        throw new Exception($"{currentStudy.Type} is not configured");

                }
                switch (studyIndex)
                {
                    case 0:
                        LastMinute.FirstStudyValue = StudyValue;
                        break;

                    case 1:
                        LastMinute.SecondStudyValue = StudyValue;
                        break;
                }
                studyIndex++;
            }
            OnPropertyChanged(nameof(TotalVelocity));
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
            if ((CurrentPosition?.Quantity ?? default(decimal)) == 0)
            {
                CanBuy = true;
                CanSell = false;
            }
            else if (CurrentPosition?.Quantity < FirstStudy.MaxSharesInPlay)
            {
                //no we can buy if needed
                CanBuy = true;
                CanSell = true;
                MaxBuy = (int)(FirstStudy.MaxSharesInPlay - CurrentPosition?.Quantity ?? default(decimal));
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
            //if there is an order pending then exit
            if (TradeApp.IsOrderPending()) { return; }
            if (CanBuy &&
                TotalVelocity > VelocityTradeOrderValue &&
                LastMinute.FirstStudyChange > 0 &&
                LastMinute.SecondStudyChange > 0 &&
                AllMinutes.Count > SecondStudy.Period
                )
            {
                VelocityTradeTrailingStopValue = VelocityTradeOrderValue;
                //buy order
                int MaxOrderSize = MaxBuy ?? FirstStudy.DefaultOrderSize;
                var orderRequest = new TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody(
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderTypes.EQ,
                    LastMinute.OrderKey,
                    Symbol,
                    MaxOrderSize,
                    Ticks.Last().Ask,
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderAction.BUY
                    );
                TradeApp.AddOrderToQueue(orderRequest);

            }
            else if (CanSell && (LastMinute.Close < CurrentPosition.HighSharePrice - VelocityTradeTrailingStopValue))
            {
                Trace.WriteLineIf(LastMinute.FirstStudyChange < 0, $"Sell : LastMinute.FirstStudyChange:{LastMinute.FirstStudyChange} < 0");

                //sell order
                var orderRequest = new TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody(
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderTypes.EQ,
                    LastMinute.OrderKey,
                    Symbol,
                    FirstStudy.MaxSharesInPlay,
                    LastMinute.Ticks.Last().Bid,
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderAction.SELL
                    );
                TradeApp.AddOrderToQueue(orderRequest);

            }
        }


        private void ProcessChartMinutes()
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(
                        () =>
                            ChartMinutes = AllMinutes.TakeLast(30).ToObservableCollection()
                        )
                    );
            }
            else
            {
                ChartMinutes = AllMinutes.TakeLast(30).ToObservableCollection();
            }

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