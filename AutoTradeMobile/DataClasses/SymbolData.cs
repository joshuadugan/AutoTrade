using System.ComponentModel;
using TradeLogic.APIModels.Quotes;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AutoTradeMobile
{
    public partial class SymbolData : ObservableObject
    {

        public SymbolData()
        {
        }


        [ObservableProperty]
        ObservableCollection<Tick> ticks = new();

        [ObservableProperty]
        ObservableCollection<Minute> minutes = new();

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
        double changeClose;

        [ObservableProperty]
        double changeClosePercentage;

        [ObservableProperty]
        long totalVolume;

        [ObservableProperty]
        double lastTrade;

        [ObservableProperty]
        Minute lastMinute;

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
                throw new NotImplementedException("not supporting AA quote type yet");
            }
            else if (quote.QuoteData.Intraday != null)
            {
                //global properties
                ChangeClose = quote.QuoteData.Intraday.ChangeClose;
                ChangeClosePercentage = quote.QuoteData.Intraday.ChangeClosePercentage;
                TotalVolume = quote.QuoteData.Intraday.TotalVolume;
                LastTrade = quote.QuoteData.Intraday.LastTrade;

                //tick properties
                t.Time = quote.QuoteData.DateTimeUTC;
                t.Ask = quote.QuoteData.Intraday.Ask;
                t.Bid = quote.QuoteData.Intraday.Bid;
                t.High = quote.QuoteData.Intraday.High;
                t.Low = quote.QuoteData.Intraday.Low;
                t.LastTrade = Math.Round(quote.QuoteData.Intraday.LastTrade, 2);

            }

            Ticks.Add(t);
            TickCount++;

            //aggrigate the tick into the minutes
            if (LastMinute == null)
            {
                LastMinute = t.ToMinute(t.LastTrade);
                Trace.WriteLine($"First Minute {t.MinuteTime}");
            }
            else if (LastMinute.TradeMinute != t.MinuteTime)
            {
                AddToMinute(LastMinute);
                LastMinute = t.ToMinute(LastMinute.Close);
            }
            else
            {
                //add to current minute
                LastMinute.AddTick(t);
                //NotifyPropertyChanged("Minutes");
            }

        }

        private void AddToMinute(Minute thisMinute)
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(() => Minutes.Add(thisMinute)));
            }
            else
            {
                Minutes.Add(thisMinute);
            }
            Trace.WriteLine($"New Minute Added To Minutes {thisMinute.TradeMinute}");
        }

        public class Tick
        {
            public long Time { get; set; }
            public string MinuteTime
            {
                get
                {
                    return DateTime.FromFileTimeUtc(Time).ToString("HH:mm");
                }
            }
            public double Ask { get; set; }
            public double Bid { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double LastTrade { get; set; }

            public Minute ToMinute(double close)
            {
                return new Minute()
                {
                    TradeMinute = MinuteTime,
                    Open = LastTrade,
                    High = LastTrade,
                    Low = LastTrade,
                    Close = LastTrade,
                    MinuteColor = LastTrade > close ? Colors.Green : Colors.Red
                };
            }
        }

        public partial class Minute : ObservableObject
        {
            [ObservableProperty]
            string tradeMinute;
            [ObservableProperty]
            double open;
            [ObservableProperty]
            double high;
            [ObservableProperty]
            double low;
            [ObservableProperty]
            double close;
            [ObservableProperty]
            Color minuteColor;
            [ObservableProperty]
            long lastTickTime;

            internal void AddTick(Tick t)
            {
                if (t.LastTrade > High) { High = t.LastTrade; }
                if (t.LastTrade < Low) { Low = t.LastTrade; }
                Close = t.LastTrade;
                LastTickTime = t.Time;
                MinuteColor = t.LastTrade > Open ? Colors.Green : Colors.Red;
                Trace.WriteLine($"Added {t.LastTrade} Tick to Minute {TradeMinute}, open {Open} - high {High} - low {Low} - close {Close}");
            }

        }

    }
}