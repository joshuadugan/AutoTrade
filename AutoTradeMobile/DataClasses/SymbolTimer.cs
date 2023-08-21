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

        public void StartTickerTimer(bool replayLastSession)
        {
            TickerTimer = new(RequestSymbolData, replayLastSession, 1000, 1000);
        }

        /// <summary>
        /// timer callback
        /// </summary>
        /// <param name="replayLastSession"></param>
        /// <exception cref="Exception"></exception>
        private void RequestSymbolData(object replayLastSession)
        {

            lock (TickerTimer)
            {

                try
                {
                    if ((bool)replayLastSession)
                    {
                        Stopwatch mockSw = Stopwatch.StartNew();
                        var qd = GetMockDataQuote();
                        if (qd == null)
                        {
                            TradingError = "No Mock Data";
                            return;
                        }
                        SymbolData.addQuote(qd);
                        mockSw.Stop();
                        LastQuoteResponseTime = mockSw.Elapsed;
                        TotalRequests += 1;
                        return;
                    }
                    //will be called by the timer to collect data about the symbol
                    Stopwatch sw = Stopwatch.StartNew();
                    GetQuotesResponse TickResult = TradeAPI.GetQuotesAsync(AccessToken, CurrentSymbolList).Result;
                    sw.Stop();
                    Trace.WriteLineIf(sw.Elapsed.TotalMilliseconds > 500, $"RequestSymbolData delay: {sw.Elapsed.TotalMilliseconds}");

                    LastQuoteResponseTime = sw.Elapsed;
                    TotalRequests += 1;

                    if (TickResult == null) throw new Exception("No Tick Result");
                    if (!CurrentSymbolList.Contains(TickResult.QuoteData.Product.Symbol.ToUpper()))
                    {
                        throw new Exception("Tick Result doesnt match symbol");
                    }

                    SymbolData.addQuote(TickResult);

                    string fileData = JsonSerializer.Serialize(TickResult.QuoteData.Intraday);
                    string symbolFileName = $"{TickResult.QuoteData.Product.Symbol.ToUpper()}.txt";
                    SymbolLogQueue.Enqueue(new LogQueueObj() { fileData = fileData, fileName = symbolFileName });

                }
                catch (Exception ex)
                {
                    TradingError = ex.Message;
                }

            }
        }

        private GetQuotesResponse GetMockDataQuote()
        {
            int rowNumber = TotalRequests;
            var data = MockDataCSV;
            if (data == null || data.Count == 0) { return null; }
            if (rowNumber >= data.Count) { TotalRequests = 0; rowNumber = 0; }
            //set the time on the return row before sending it back. this makes it look like the request is current

            var responseData = data[rowNumber];
            responseData.QuoteData.DateTimeUTC = DateTime.Now.ToFileTimeUtc();
            responseData.QuoteData.DateTime = DateTime.Now.ToString();
            return responseData;
        }

        static Object _mockDataLock = new Object();

        static List<GetQuotesResponse> mockResponseData;
        private List<GetQuotesResponse> MockDataCSV
        {
            get
            {
                lock (_mockDataLock)
                {
                    try
                    {
                        if (mockResponseData == null)
                        {
                            mockResponseData = new List<GetQuotesResponse>();
                            string symbol = CurrentSymbolList.First();
                            string fileData = Helpers.ReadTextFileAsync($"{symbol}.txt").Result;
                            List<string> lines = fileData.Split(Environment.NewLine).ToList();
                            foreach (string line in lines)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        //rows are intraday json
                                        var Intraday = JsonSerializer.Deserialize<Intraday>(line);

                                        var qr = new GetQuotesResponse()
                                        {
                                            QuoteData = new QuoteData()
                                            {
                                                AhFlag = false,
                                                DateTimeUTC = DateTime.Now.ToFileTimeUtc(),
                                                DateTime = DateTime.Now.ToString(),
                                                QuoteStatus = "REALTIME",
                                                Product = new TradeLogic.APIModels.Quotes.Product()
                                                {
                                                    SecurityType = "EQ",
                                                    Symbol = symbol
                                                },
                                                Intraday = Intraday

                                            }
                                        };
                                        mockResponseData.Add(qr);
                                    }

                                }
                                catch (Exception)
                                {
                                    //skip the line
                                }

                            }

                        }
                        return mockResponseData;
                    }
                    catch (Exception)
                    {
                        StopTrading();
                        throw;
                    }
                }
            }
        }






    }
}