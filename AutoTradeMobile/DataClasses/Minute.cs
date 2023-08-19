using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace AutoTradeMobile
{
    public partial class SymbolData
    {

        public partial class Minute : ObservableObject
        {
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
            DateTime lastTickTime;
            [ObservableProperty]
            double averageTrade;
            [ObservableProperty] 
            double firstStudyValue;
            [ObservableProperty]
            double secondStudyValue;

            public List<Tick> Ticks { get; private set; } = new();

            internal void AddTick(Tick t)
            {
                if (t.Ask > High) { High = t.Ask; }
                if (t.Bid < Low) { Low = t.Bid; }
                Close = t.LastTrade;
                LastTickTime = t.Time;
                MinuteColor = t.LastTrade > Open ? Colors.Green : Colors.Red;
                Ticks.Add(t);
                AverageTrade = Ticks.Average(t => t.LastTrade);
                Trace.WriteLine($"Added {t.LastTrade} Tick to Minute ({Ticks.Count}) {TradeMinute}, open {Open} - high {High} - low {Low} - close {Close} - AverageTrade {AverageTrade}");
            }

        }


    }

}