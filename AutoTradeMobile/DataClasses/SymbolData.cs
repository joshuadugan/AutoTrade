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
                    UptrendAmountRequired = 0.1m,
                    Type = StudyConfig.StudyType.ALMA
                });
                Studies.Add(new StudyConfig()
                {
                    Period = 20,
                    UptrendAmountRequired = 0.0m,
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
        ObservableCollection<ChartMinute> chartData = new();

        public List<Minute> AllMinutes { get; private set; } = new();
        public List<StudyChartValue> AllFirstStudyValues { get; private set; } = new();
        public List<StudyChartValue> AllSecondStudyValues { get; private set; } = new();

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
        decimal velocityTradeOrderValue = .25m;

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

        [ObservableProperty]
        Color sixtyTicksChangeColor;

        [ObservableProperty]
        decimal totalVelocity;

        [ObservableProperty]
        Color totalVelocityColor;

        [ObservableProperty]
        decimal firstStudyValue;

        [ObservableProperty]
        decimal firstStudyPreviousValue;

        [ObservableProperty]
        decimal firstStudyChange;

        [ObservableProperty]
        Color firstStudyColor;

        [ObservableProperty]
        decimal secondStudyValue;

        [ObservableProperty]
        decimal secondStudyPreviousValue;

        [ObservableProperty]
        decimal secondStudyChange;

        [ObservableProperty]
        Color secondStudyColor;

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
            SixtyTicksChangeColor = SixtyTicksChange >= 0 ? Colors.Green : Colors.Red;

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
            if (AllMinutes.Count < 2) { return; }//need two minutes of data

            int studyIndex = 0;
            foreach (var currentStudy in Studies)
            {
                var quotes = AllMinutes;
                var lookbackPeriods = currentStudy.Period;
                var defaultValue = LastMinute.AverageTrade;
                List<decimal> thisStudyValues = new List<decimal>() { LastMinute.Close, LastMinute.Close };
                switch (currentStudy.Type)
                {
                    case StudyConfig.StudyType.SMA:
                        thisStudyValues = quotes.GetSma(lookbackPeriods).TakeLast(2).Select(val => val.Sma.ToDecimal() ?? defaultValue).ToList();
                        break;
                    case StudyConfig.StudyType.EMA:
                        thisStudyValues = quotes.GetEma(lookbackPeriods).TakeLast(2).Select(val => val.Ema.ToDecimal() ?? defaultValue).ToList();
                        break;
                    case StudyConfig.StudyType.VWMA:
                        thisStudyValues = quotes.GetVwma(lookbackPeriods).TakeLast(2).Select(val => val.Vwma.ToDecimal() ?? defaultValue).ToList();
                        break;
                    case StudyConfig.StudyType.ALMA:
                        thisStudyValues = quotes.GetAlma(lookbackPeriods, offset: 0.85, sigma: 6).TakeLast(2).Select(val => val.Alma.ToDecimal() ?? defaultValue).ToList();
                        break;
                    default:
                        throw new Exception($"{currentStudy.Type} is not configured");

                }
                switch (studyIndex)
                {
                    case 0:
                        FirstStudyValue = thisStudyValues[0];
                        FirstStudyPreviousValue = thisStudyValues[1];
                        FirstStudyChange = FirstStudyPreviousValue - FirstStudyValue;
                        FirstStudyColor = FirstStudyChange >= 0 ? Colors.Green : Colors.Red;
                        break;

                    case 1:
                        SecondStudyValue = thisStudyValues[0];
                        SecondStudyPreviousValue = thisStudyValues[1];
                        SecondStudyChange = SecondStudyPreviousValue - SecondStudyValue;
                        SecondStudyColor = FirstStudyChange >= 0 ? Colors.Green : Colors.Red;
                        break;
                }
                studyIndex++;
            }
            TotalVelocity = FirstStudyChange + SecondStudyChange;
            TotalVelocityColor = TotalVelocity >= 0 ? Colors.Green : Colors.Red;

            UpdateFirstStudyChartValue();
            UpdateSecondStudyChartValue();

        }

        private void UpdateFirstStudyChartValue()
        {
            var last = AllFirstStudyValues.LastOrDefault();
            if (last == null || last.Time != LastMinute.TradeMinute)
            {
                AllFirstStudyValues.Add(new StudyChartValue()
                {
                    Time = LastMinute.TradeMinute,
                    Value = FirstStudyValue,
                });
            }
            else
            {
                last.Value = FirstStudyValue;
            }
        }
        private void UpdateSecondStudyChartValue()
        {
            var last = AllSecondStudyValues.LastOrDefault();
            if (last == null || last.Time != LastMinute.TradeMinute)
            {
                AllSecondStudyValues.Add(new StudyChartValue()
                {
                    Time = LastMinute.TradeMinute,
                    Value = SecondStudyValue,
                });
            }
            else
            {
                last.Value = SecondStudyValue;
            }
        }

        public class StudyChartValue
        {
            public String Time { get; set; }
            public decimal Value { get; set; }
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
                AllMinutes.Count > SecondStudy.Period
                )
            {
                VelocityTradeTrailingStopValue = VelocityTradeOrderValue * 2;
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
            else if (CanSell && (LastMinute.Close < CurrentPosition.TrailingStopPrice))
            {

                //sell order
                var orderRequest = new TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody(
                    TradeLogic.APIModels.Orders.PreviewOrderResponse.RequestBody.OrderTypes.EQ,
                    LastMinute.OrderKey,
                    Symbol,
                    FirstStudy.MaxSharesInPlay,
                    CurrentPosition.TrailingStopPrice,
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
                        {
                            ProcessChartMinute_Internal();
                        })
                    );
            }
            else
            {
                ProcessChartMinute_Internal();
            }

        }

        private void ProcessChartMinute_Internal()
        {
            var mergedChartData = AllMinutes
                            .Join(AllFirstStudyValues, m => m.TradeMinute, s => s.Time, (tm, fs) => new { tm, fs })
                            .Join(AllSecondStudyValues, j => j.tm.TradeMinute, s => s.Time, (j, ss) => new
                            {
                                minute = new ChartMinute(j.tm)
                                {
                                    FirstStudyValue = j.fs.Value,
                                    SecondStudyValue = ss.Value
                                }
                            });

            ChartData = mergedChartData
                            .Select(mcd => mcd.minute)
                            .TakeLast(SecondStudy.Period).ToObservableCollection();
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