﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TradeLogic.APIModels.Quotes;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {

        private void RequestSymbolData_Simulated(object args)
        {
            lock (TickerTimer)
            {
                try
                {
                    Stopwatch mockSw = Stopwatch.StartNew();
                    var qd = GetMockDataQuote();
                    if (qd == null)
                    {
                        TradingError = "No Mock Data";
                        return;
                    }
                    Symbol.addQuote(qd);
                    mockSw.Stop();
                    LastQuoteResponseTime = mockSw.Elapsed;
                    TotalRequests += 1;
                    return;

                }
                catch (Exception ex)
                {
                    TradingError = ex.Message;
                    ex.WriteExceptionToLog();
                }
            }
        }

        private DateTime SimTradeTime { get; set; } = DateTime.Now;
        private GetQuotesResponse GetMockDataQuote()
        {
            int rowNumber = TotalRequests;
            var data = MockDataCSV;
            if (data == null || data.Count == 0) { return null; }
            if (rowNumber >= data.Count) { StopTrading(); rowNumber = data.Count - 1; }
            //set the time on the return row before sending it back. this makes it look like the request is current

            var responseData = data[rowNumber];
            responseData.QuoteData.DateTimeUTC = SimTradeTime.AddSeconds(rowNumber).ToFileTimeUtc();
            responseData.QuoteData.DateTime = SimTradeTime.AddSeconds(rowNumber).ToString();
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
                            string fileData = Helpers.ReadTextFile($"{SymbolName}.txt");
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
                                                    Symbol = SymbolName
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
