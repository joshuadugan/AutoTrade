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
            public Minute(Tick firstTick, Minute lastMinute)
            {
                var firstTime = firstTick.Time;
                MinuteDateTime = new DateTime(firstTime.Year, firstTime.Month, firstTime.Day, firstTime.Hour, firstTime.Minute, 0);//time to the minute
                TradeMinute = MinuteDateTime.ToString("HH:mm");
                startingVolume = firstTick.Volume;
                Open = firstTick.LastTrade;
                High = firstTick.LastTrade;
                Low = firstTick.LastTrade;
                FirstStudyValue = firstTick.LastTrade;
                SecondStudyValue = firstTick.LastTrade;
                FirstStudyStartingValue = lastMinute?.FirstStudyValue ?? firstTick.LastTrade;
                SecondStudyStartingValue = lastMinute?.SecondStudyValue ?? firstTick.LastTrade;
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
            [NotifyPropertyChangedFor(nameof(FirstStudyChange))]
            [NotifyPropertyChangedFor(nameof(SecondStudyChange))]
            [NotifyPropertyChangedFor(nameof(FirstStudyColor))]
            [NotifyPropertyChangedFor(nameof(SecondStudyColor))]
            [NotifyPropertyChangedFor(nameof(TotalVelocity))]
            [NotifyPropertyChangedFor(nameof(TotalVelocityColor))]
            [ObservableProperty]
            decimal close;

            [ObservableProperty]
            decimal averageTrade;

            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(TotalVelocity))]
            [NotifyPropertyChangedFor(nameof(TotalVelocityColor))]
            decimal firstStudyValue;

            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(TotalVelocity))]
            [NotifyPropertyChangedFor(nameof(TotalVelocityColor))]
            decimal secondStudyValue;

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

            public decimal FirstStudyChange
            {
                get
                {
                    return FirstStudyValue - FirstStudyStartingValue;
                }
            }

            public decimal SecondStudyChange
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

            public decimal FirstStudyStartingValue { get; set; }
            public decimal SecondStudyStartingValue { get; set; }

            public decimal TotalVelocity
            {
                get
                {
                    return MinuteChange + FirstStudyChange + SecondStudyChange;
                }
            }

            public Color TotalVelocityColor
            {
                get
                {
                    return TotalVelocity >= 0 ? Colors.Green : Colors.Red;
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