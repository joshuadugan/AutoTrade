using System.ComponentModel;
using TradeLogic.APIModels.Quotes;

namespace AutoTradeMobile
{
    internal class SymbolData
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

        string _Symbol = "No Data Received Yet";
        public string Symbol { get { return _Symbol; } }

        public int TickCount { get { return ticks.Count; } }

        public double LastPrice { get; set; }
        public Int64 LastTime { get; set; }

        public void addQuote(GetQuotesResponse quote)
        {
            if (ticks.Count == 0)
            {
                _Symbol = quote.QuoteData.Product.Symbol.ToUpper();
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