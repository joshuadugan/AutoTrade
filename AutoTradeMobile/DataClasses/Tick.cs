﻿namespace AutoTradeMobile
{
    public partial class SymbolData
    {
        public class Tick
        {
            public DateTime Time { get; set; }
            public string MinuteTime
            {
                get
                {
                    return Time.ToString("HH:mm");
                }
            }
            public double Ask { get; set; }
            public double Bid { get; set; }
            public double LastTrade { get; set; }

            public Minute ToMinute(double NewOpen, double firstStudyValue, double secondStudyValue)
            {
                return new Minute()
                {
                    LastTickTime = Time,
                    TradeMinute = MinuteTime,
                    Open = NewOpen,
                    High = Ask,
                    Low = Bid,
                    Close = LastTrade,
                    AverageTrade = LastTrade,
                    FirstStudyValue = firstStudyValue,
                    SecondStudyValue = secondStudyValue,
                    MinuteColor = LastTrade > NewOpen ? Colors.Green : Colors.Red
                };
            }
        }


    }

}