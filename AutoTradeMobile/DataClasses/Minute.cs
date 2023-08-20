using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace AutoTradeMobile
{
    public partial class SymbolData
    {

        public partial class Minute : ObservableObject
        {

            [ObservableProperty]
            StudyConfig firstStudy;
            [ObservableProperty]
            StudyConfig secondStudy;
            [ObservableProperty]
            string tradeMinute;
            [ObservableProperty]
            double open;
            [ObservableProperty]
            double high;
            [ObservableProperty]
            double low;
            [ObservableProperty]
            double close;
            [ObservableProperty]
            Color minuteColor;
            [ObservableProperty]
            double minuteChange;
            [ObservableProperty]
            DateTime lastTickTime;
            [ObservableProperty]
            double averageTrade;
            [ObservableProperty]
            double firstStudyValue;
            [ObservableProperty]
            double secondStudyValue;
            [ObservableProperty]
            double firstStudyChange;
            [ObservableProperty]
            double secondStudyChange;
            [ObservableProperty]
            double firstStudyChangePercentage;
            [ObservableProperty]
            double secondStudyChangePercentage;
            [ObservableProperty]
            Color firstStudyColor;
            [ObservableProperty]
            Color secondStudyColor;

            public List<Tick> Ticks { get; private set; } = new();
            public double FirstStudyStartingValue { get; set; }
            public double SecondStudyStartingValue { get; set; }
            public string OrderKey
            {
                get
                {
                    return $"{TradeMinute}_{DateTime.Now.ToShortDateString}";
                }
            }

            internal void AddTick(Tick t)
            {
                if (t.Ask > High) { High = t.Ask; }
                if (t.Bid < Low) { Low = t.Bid; }
                Close = t.LastTrade;
                LastTickTime = t.Time;
                MinuteColor = t.LastTrade > Open ? Colors.Green : Colors.Red;
                Ticks.Add(t);
                AverageTrade = Ticks.Average(t => t.LastTrade);

                MinuteChange = Close - Open;

                //process the ticks study calcs
                FirstStudyChange = FirstStudyValue - FirstStudyStartingValue;
                SecondStudyChange = SecondStudyValue - SecondStudyStartingValue;

                FirstStudyColor = FirstStudyChange >= 0 ? Colors.Green : Colors.Red;
                SecondStudyColor = SecondStudyChange >= 0 ? Colors.Green : Colors.Red;

                //Trace.WriteLine($"Added {t.LastTrade} Tick to Minute ({Ticks.Count}) {TradeMinute}, open {Open} - high {High} - low {Low} - close {Close} - AverageTrade {AverageTrade}");
                //Trace.WriteLine($"Study Data for Minute {TradeMinute}, First : value {FirstStudyValue} - change {FirstStudyChange}");
                //Trace.WriteLine($"Study Data for Minute {TradeMinute}, Second : value {SecondStudyValue} - change {SecondStudyChange}");

            }

        }


    }

}