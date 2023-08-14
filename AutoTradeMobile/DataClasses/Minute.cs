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
            long lastTickTime;

            internal void AddTick(Tick t)
            {
                if (t.LastTrade > High) { High = t.LastTrade; }
                if (t.LastTrade < Low) { Low = t.LastTrade; }
                Close = t.LastTrade;
                LastTickTime = t.Time;
                MinuteColor = t.LastTrade > Open ? Colors.Green : Colors.Red;
                Trace.WriteLine($"Added {t.LastTrade} Tick to Minute {TradeMinute}, open {Open} - high {High} - low {Low} - close {Close}");
            }

        }


    }

}