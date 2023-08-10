using System.Diagnostics;
using TradeLogic.APIModels.Quotes;

namespace AutoTradeMobile
{
    internal partial class TradeApp
    {

        /// <summary>
        /// timer callback
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="Exception"></exception>
        private static void RequestSymbolData(object state)
        {
            try
            {

                //will be called by the timer to collect data about the symbol
                Stopwatch sw = Stopwatch.StartNew();
                GetQuotesResponse TickResult = trader.GetQuotes(accessToken, currentSymbolList).Result;
                sw.Stop();

                SessionData.LastQuoteResponseTime = sw.Elapsed;
                SessionData.LastQuoteDate = DateTime.Now;
                SessionData.TotalRequests += 1;

                if (TickResult == null) throw new Exception("No Tick Result");
                if (!currentSymbolList.Contains(TickResult.QuoteData.Product.Symbol.ToUpper()))
                {
                    throw new Exception("Tick Result doesnt match symbol");
                }

                var thisSymbolData = GetSymbolData(TickResult.QuoteData.Product.Symbol);
                thisSymbolData.addQuote(TickResult);

            }
            catch (Exception ex)
            {
                StopTrading();
                SessionData.TradingError = ex.Message;
            }

        }
    }
}