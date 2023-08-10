using System.ComponentModel;
using TradeLogic.APIModels.Quotes;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoTradeMobile
{
    internal class SymbolData : ObservableObject
    {
        BindingList<Tick> ticks;
        
        public SymbolData()
        {
            ticks = new();
            ticks.AllowNew = true;
            ticks.AllowRemove = false;
            ticks.RaiseListChangedEvents = true;
            ticks.AllowEdit = false;
        }

        private string _Symbol = "No Data Received Yet";
        public string Symbol
        {
            get => _Symbol;
            set => SetProperty(ref _Symbol, value);
        }

        public int TickCount { 
            get => ticks.Count; 
        }

        double _LastPrice;
        public double LastPrice {
            get => _LastPrice;
            set => SetProperty(ref _LastPrice, value);
        }

        Int64 _LastTime;
        public Int64 LastTime {
            get => _LastTime;
            set => SetProperty(ref _LastTime, value);
        }

        bool _IsAfterHours;
        public bool IsAfterHours {
            get => _IsAfterHours;
            set => SetProperty(ref _IsAfterHours, value);
        }

        string _QuoteStatus;
        public string QuoteStatus {
            get => _QuoteStatus;
            set => SetProperty(ref _QuoteStatus, value);
        }

        public void addQuote(GetQuotesResponse quote)
        {
            if (ticks.Count == 0)
            {
                Symbol = quote.QuoteData.Product.Symbol.ToUpper();
            }
            IsAfterHours = quote.QuoteData.AhFlag;
            QuoteStatus = quote.QuoteData.QuoteStatus;

            if (quote.QuoteData.All == null)
            {
                throw new InvalidOperationException("Cannor process quote, missing quote.QuoteData.All");
            }

            Tick t = new();
            t.price = quote.QuoteData.All.LastTrade;
            t.time = quote.QuoteData.All.TimeOfLastTrade;

            LastPrice = t.price;
            LastTime = t.time;

            ticks.Add(t);

        }

        public class Tick
        {
            public double price { get; set; }
            public Int64 time { get; set; }
        }

    }
}