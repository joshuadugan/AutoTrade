using System.Diagnostics;
using TradeLogic.APIModels.Quotes;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {

        /// <summary>
        /// timer callback
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="Exception"></exception>
        private void RequestSymbolData(object state)
        {
            try
            {
                if (currentSymbolList.Contains("DEMO"))
                {
                    Stopwatch mockSw = Stopwatch.StartNew();
                    var qd = GetMockDataQuote(SessionData.TotalRequests);
                    var mockSymbolData = GetSymbolData("DEMO");
                    mockSymbolData.addQuote(qd);
                    mockSw.Stop();
                    SessionData.LastQuoteResponseTime = mockSw.Elapsed;
                    SessionData.TotalRequests += 1;
                    return;
                }
                Trace.WriteLine("RequestSymbolData Start");
                //will be called by the timer to collect data about the symbol
                Stopwatch sw = Stopwatch.StartNew();
                GetQuotesResponse TickResult = trader.GetQuotes(accessToken, currentSymbolList).Result;
                sw.Stop();
                Trace.WriteLine($"RequestSymbolData End: {sw.Elapsed.ToString("fff")}");

                SessionData.LastQuoteResponseTime = sw.Elapsed;
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

        private GetQuotesResponse GetMockDataQuote(int rowNumber)
        {
            var data = MockDataCSV;
            if (data == null) { throw new Exception("No Mock Data"); }
            if (rowNumber > data.Count) { throw new Exception("Data Exhausted"); }
            //set the time on the return row before sending it back. this makes it look like the request is current

            var responseData = data[rowNumber];
            responseData.QuoteData.DateTimeUTC = DateTime.Now.ToFileTimeUtc();
            responseData.QuoteData.DateTime = DateTime.Now.ToString();
            return responseData;
        }

        static List<GetQuotesResponse> mockResponseData;
        private List<GetQuotesResponse> MockDataCSV
        {
            get
            {
                try
                {
                    if (mockResponseData == null)
                    {
                        mockResponseData = new List<GetQuotesResponse>();
                        var file = LoadCSVAsset().Result;
                        foreach (var row in file)
                        {
                            //Date,Open,High,Low,Close,Adj Close,Volume
                            string Date = row[0];
                            double Open = double.Parse(row[1]);
                            double High = double.Parse(row[2]);
                            double Low = double.Parse(row[3]);
                            double Close = double.Parse(row[4]);
                            long Volume = long.Parse(row[6]);
                            var qr = new GetQuotesResponse()
                            {
                                QuoteData = new QuoteData()
                                {
                                    AhFlag = false,
                                    DateTimeUTC = DateTime.Now.ToFileTimeUtc(),
                                    DateTime = DateTime.Now.ToString(),
                                    QuoteStatus = "REALTIME",
                                    Product = new Product()
                                    {
                                        SecurityType = "EQ",
                                        Symbol = "DEMO"
                                    },
                                    Intraday = new Intraday()
                                    {
                                        Ask = Open,
                                        Bid = Open - 1,
                                        High = High,
                                        Low = Low,
                                        LastTrade = Open,
                                        TotalVolume = Volume,
                                        ChangeClose = High - Low,
                                        ChangeClosePercentage = High - Low / 100,
                                        CompanyName = "DEMO"
                                    }

                                }
                            };
                            mockResponseData.Add(qr);
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
        async Task<List<string[]>> LoadCSVAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("TSLA.csv");

            List<string[]> dataList = new List<string[]>();

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] data = line.Split(',');
                    dataList.Add(data);
                }
            }

            return dataList;
        }

    }
}