using System.ComponentModel;
using TradeLogic.APIModels.Quotes;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AutoTradeMobile.DataClasses;
using System.Globalization;

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

        [ObservableProperty]
        double todayHigh;

        [ObservableProperty]
        double todayLow;

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
                throw new NotImplementedException("not supporting All quote type yet");
            }
            else if (quote.QuoteData.Intraday != null)
            {
                //global properties
                ChangeClose = quote.QuoteData.Intraday.ChangeClose;
                ChangeClosePercentage = quote.QuoteData.Intraday.ChangeClosePercentage;
                TotalVolume = quote.QuoteData.Intraday.TotalVolume;
                LastTrade = quote.QuoteData.Intraday.LastTrade;
                TodayLow = quote.QuoteData.Intraday.Low;
                TodayHigh = quote.QuoteData.Intraday.High;

                //tick properties
                string format = "HH:mm:ss EDT MM-dd-yyyy";
                DateTime dateTime;
                if (DateTime.TryParseExact(quote.QuoteData.DateTime, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime) == false)
                {
                    if (DateTime.TryParse(quote.QuoteData.DateTime, out dateTime) == false)
                    {
                        dateTime = DateTime.Now;
                    }
                }

                t.Time = dateTime;
                t.Ask = quote.QuoteData.Intraday.Ask;
                t.Bid = quote.QuoteData.Intraday.Bid;
                t.LastTrade = quote.QuoteData.Intraday.LastTrade;
            }

            Ticks.Add(t);
            TickCount++;

            //aggrigate the tick into the minutes
            if (LastMinute == null || LastMinute.TradeMinute != t.MinuteTime)
            {
                double NewOpen = t.LastTrade;
                if (LastMinute != null)
                {
                    NewOpen = LastMinute.AverageTrade;
                }
                LastMinute = t.ToMinute(NewOpen);
                AddToMinutes(LastMinute);
            }
            else
            {
                LastMinute.AddTick(t);
                RefreshLastMinute();
            }

            var currentStudy = Studies.FirstOrDefault();
            if (currentStudy != null)
            {
                var StudyData = Minutes.TakeLast(currentStudy.Period);
                switch (currentStudy.Field)
                {
                    case StudyConfig.FieldName.open:
                        LastMinute.StudyValue = StudyData.Select(sd => sd.Open).Average();
                        break;
                    case StudyConfig.FieldName.high:
                        LastMinute.StudyValue = StudyData.Select(sd => sd.High).Average();
                        break;
                    case StudyConfig.FieldName.low:
                        LastMinute.StudyValue = StudyData.Select(sd => sd.Low).Average();
                        break;
                    case StudyConfig.FieldName.close:
                        LastMinute.StudyValue = StudyData.Select(sd => sd.Close).Average();
                        break;
                    default:
                        break;
                }
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
            Trace.WriteLine($"New Minute {thisMinute.TradeMinute}");
        }


    }

}