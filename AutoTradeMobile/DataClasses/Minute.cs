using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using TradeLogic.APIModels.Accounts.portfolio;

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
            DateTime lastTickTime;

            [ObservableProperty]
            double averageTrade;

            [ObservableProperty]
            double firstStudyValue;

            [ObservableProperty]
            double secondStudyValue;

            [NotifyPropertyChangedFor(nameof(MinuteColor))]
            [NotifyPropertyChangedFor(nameof(MinuteChange))]
            [NotifyPropertyChangedFor(nameof(FirstStudyChange))]
            [NotifyPropertyChangedFor(nameof(SecondStudyChange))]
            [NotifyPropertyChangedFor(nameof(FirstStudyColor))]
            [NotifyPropertyChangedFor(nameof(SecondStudyColor))]
            [ObservableProperty]
            double close;

            public Color MinuteColor
            {
                get
                {
                    return Close > Open ? Colors.Green : Colors.Red;
                }
            }

            public double MinuteChange
            {
                get
                {
                    return Close - Open;
                }
            }

            public double FirstStudyChange
            {
                get
                {
                    return FirstStudyValue - FirstStudyStartingValue;
                }
            }

            public double SecondStudyChange
            {
                get
                {
                    return SecondStudyValue - SecondStudyStartingValue;
                }
            }

            public Color FirstStudyColor
            {
                get
                {
                    return FirstStudyChange >= 0 ? Colors.Green : Colors.Red;
                }
            }

            public Color SecondStudyColor
            {
                get
                {
                    return SecondStudyChange >= 0 ? Colors.Green : Colors.Red;
                }
            }

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
                Ticks.Add(t);
                AverageTrade = Ticks.Average(t => t.LastTrade);

                //Trace.WriteLine($"Added {t.LastTrade} Tick to Minute ({Ticks.Count}) {TradeMinute}, open {Open} - high {High} - low {Low} - close {Close} - AverageTrade {AverageTrade}");
                //Trace.WriteLine($"Study Data for Minute {TradeMinute}, First : value {FirstStudyValue} - change {FirstStudyChange}");
                //Trace.WriteLine($"Study Data for Minute {TradeMinute}, Second : value {SecondStudyValue} - change {SecondStudyChange}");

            }

        }


    }

}