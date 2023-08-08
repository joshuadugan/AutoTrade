using System.ComponentModel;
using TradeLogic.APIModels.Quotes;

namespace AutoTradeMobile
{
    internal class SymbolData
    {
        BindingList<tick> ticks;

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

        public Int64 TickCount { get { return ticks.Count; } }

        public void addQuote(GetQuotesResponse quote)
        {
            if (ticks.Count == 0)
            {
                _Symbol = quote.QuoteData.Product.Symbol;
            }

            tick t = new();
            t.price = quote.QuoteData.All.LastTrade;
            t.time = quote.QuoteData.All.TimeOfLastTrade;
            ticks.Add(t);

        }

        public class tick
        {
            public double price { get; set; }
            public Int64 time { get; set; }
        }

    }
}