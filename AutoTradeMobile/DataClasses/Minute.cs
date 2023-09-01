using CommunityToolkit.Mvvm.ComponentModel;
using Skender.Stock.Indicators;
using System.Diagnostics;
using TradeLogic.APIModels.Accounts.portfolio;

namespace AutoTradeMobile
{
    public partial class SymbolData
    {

        public partial class Minute : ObservableObject, IQuote
        {
            public Minute() { }
            public Minute(Tick firstTick)
            {
                var firstTime = firstTick.Time;
                MinuteDateTime = new DateTime(firstTime.Year, firstTime.Month, firstTime.Day, firstTime.Hour, firstTime.Minute, 0);//time to the minute
                TradeMinute = MinuteDateTime.ToString("HH:mm");
                startingVolume = firstTick.Volume;
                Open = firstTick.LastTrade;
                High = firstTick.LastTrade;
                Low = firstTick.LastTrade;
                AddTick(firstTick);
            }

            public List<Tick> Ticks { get; private set; } = new();
            public DateTime MinuteDateTime { get; private set; }
            public string TradeMinute { get; private set; }


            [ObservableProperty]
            decimal open;

            [ObservableProperty]
            decimal high;

            [ObservableProperty]
            decimal low;

            [ObservableProperty]
            DateTime lastTickTime;

            [NotifyPropertyChangedFor(nameof(MinuteColor))]
            [NotifyPropertyChangedFor(nameof(MinuteChange))]
            [ObservableProperty]
            decimal close;

            [ObservableProperty]
            decimal averageTrade;

            public Color MinuteColor
            {
                get
                {
                    return Close > Open ? Colors.Green : Colors.Red;
                }
            }

            public decimal MinuteChange
            {
                get
                {
                    return Close - Open;
                }
            }

            public string OrderKey
            {
                get
                {
                    return $"{TradeMinute}_{DateTime.Now.ToShortDateString()}";
                }
            }

            decimal startingVolume;
            decimal endingVolume;

            public decimal Volume
            {
                get
                {
                    return endingVolume - startingVolume;
                }
            }

            public DateTime Date => MinuteDateTime;


            internal void AddTick(Tick t)
            {
                if (t.LastTrade > High) { High = t.LastTrade; }
                if (t.LastTrade < Low) { Low = t.LastTrade; }
                endingVolume = t.Volume;
                Close = t.LastTrade;
                LastTickTime = t.Time;
                Ticks.Add(t);
                AverageTrade = Ticks.Average(t => t.LastTrade);

            }

        }


    }

}