using System.ComponentModel;
using TradeLogic.APIModels.Quotes;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace AutoTradeMobile
{
    public partial class SymbolData : ObservableObject
    {
        public SymbolData()
        {
            ticks = new();
            ticks.AllowNew = true;
            ticks.AllowRemove = false;
            ticks.RaiseListChangedEvents = true;
            ticks.AllowEdit = false;
        }
        public BindingList<Tick> ticks { get; }

        [ObservableProperty]
        private string _Symbol = "No Data Received Yet";

        [ObservableProperty]
        int tickCount;

        [ObservableProperty]
        Int64 lastTime;

        [ObservableProperty]
        bool isAfterHours;

        [ObservableProperty]
        string quoteStatus;

        [ObservableProperty]
        double changeClose;

        [ObservableProperty]
        double changeClosePercentage;

        [ObservableProperty]
        Int64 totalVolume;

        [ObservableProperty]
        double lastTrade;

        public void addQuote(GetQuotesResponse quote)
        {
            if (ticks.Count == 0)
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
                t.LastTrade = quote.QuoteData.Intraday.LastTrade;

            }

            ticks.Add(t);
            TickCount++;

        }

        public class Tick
        {
            public Int64 Time { get; set; }
            public double Ask { get; set; }
            public double Bid { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double LastTrade { get; set; }
        }

    }
}