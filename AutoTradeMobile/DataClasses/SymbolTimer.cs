using AutoTradeMobile.DataClasses;
using CommunityToolkit.Maui.Core.Extensions;
using System.Diagnostics;
using System.Text.Json;
using TradeLogic.APIModels.Orders;
using TradeLogic.APIModels.Quotes;
using TradeLogic.ViewModels;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        private Timer TickerTimer { get; set; }

        public void StartTickerTimer()
        {
            if (ReplayLastSession)
            {
                TickerTimer = new(RequestSymbolData_Simulated, null, 1000, 100);
            }
            else
            {
                TickerTimer = new(RequestSymbolData, null, 1000, 1000);
            }
        }

        private void RequestSymbolData(object args)
        {

            lock (TickerTimer)
            {

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    //will be called by the timer to collect data about the symbol
                    GetQuotesResponse TickResult = TradeAPI.GetQuotesAsync(AccessToken, new List<string>() { SymbolName }).Result;

                    if (TickResult == null) throw new Exception("No Tick Result");
                    if (!SymbolName.Equals(TickResult.QuoteData.Product.Symbol.ToUpper())) { throw new Exception("Tick Result does not match symbol");                    }

                    Symbol.addQuote(TickResult);

                    string fileData = JsonSerializer.Serialize(TickResult.QuoteData.Intraday);
                    string symbolFileName = $"{DateTime.Today.Day}_{TickResult.QuoteData.Product.Symbol.ToUpper()}.txt";
                    LogToFile(fileData, symbolFileName);

                }
                catch (Exception ex)
                {
                    TradingError = ex.Message;
                    ex.WriteExceptionToLog();
                }
                finally
                {
                    sw.Stop();
                    Trace.WriteLineIf(sw.Elapsed.TotalMilliseconds > 500, $"RequestSymbolData delay: {sw.Elapsed.TotalMilliseconds}");
                    LastQuoteResponseTime = sw.Elapsed;
                    TotalRequests += 1;
                }

            }
        }







    }
}