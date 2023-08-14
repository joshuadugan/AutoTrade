using System.ComponentModel;
using TradeLogic.APIModels.Quotes;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AutoTradeMobile.DataClasses;

namespace AutoTradeMobile
{
    public partial class SymbolData : ObservableObject
    {
        const string StudiesFileName = "Studies.txt";
        static Object _studiesLoadLock = new Object();

        public SymbolData()
        {
            lock (_studiesLoadLock)
            {
                //load up the saved studies
                Trace.WriteLine("Loading Studies from file");
                Studies.LoadFromFile(StudiesFileName);
                Trace.WriteLine($"{Studies.Count} Studies");
                if (Studies.Count == 0)
                {
                    Trace.WriteLine("Adding default Study Config");
                    Studies.Add(new StudyConfig());
                    Studies.PersistToFile(StudiesFileName);
                }
            }

        }

        [ObservableProperty]
        ObservableCollection<Tick> ticks = new();

        [ObservableProperty]
        ObservableCollection<Minute> minutes = new();

        [ObservableProperty]
        ObservableCollection<StudyConfig> studies = new();

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
            if (LastMinute == null || LastMinute.TradeMinute != t.MinuteTime)
            {
                LastMinute = t.ToMinute(t.LastTrade);
                AddToMinutes(LastMinute);
                Trace.WriteLine($"New Minute {t.MinuteTime}");
            }
            else
            {
                LastMinute.AddTick(t);
                RefreshLastMinute();
            }

        }

        private void RefreshLastMinute()
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(
                        () =>
                        Minutes[Minutes.Count - 1] = LastMinute
                        )
                    );
            }
            else
            {
                Minutes[Minutes.Count - 1] = LastMinute;
            }

        }

        private void RemoveMinute(Minute thisMinute)
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(() => Minutes.Remove(thisMinute)));
            }
            else
            {
                Minutes.Remove(thisMinute);
            }
            Trace.WriteLine($"Minute Removed Minutes {thisMinute.TradeMinute}");
        }

        private void AddToMinutes(Minute thisMinute)
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


    }

}