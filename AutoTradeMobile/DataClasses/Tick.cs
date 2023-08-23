namespace AutoTradeMobile
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

            public Minute ToMinute(Minute lastMinute)
            {
                var NewOpen = lastMinute?.AverageTrade ?? LastTrade;
                return new Minute()
                {
                    LastTickTime = Time,
                    TradeMinute = MinuteTime,
                    Open = NewOpen,
                    High = Ask,
                    Low = Bid,
                    Close = LastTrade,
                    AverageTrade = LastTrade,
                    FirstStudyValue = lastMinute?.FirstStudyValue ?? LastTrade,
                    SecondStudyValue = lastMinute?.SecondStudyValue ?? LastTrade,
                    FirstStudyStartingValue = lastMinute?.FirstStudyValue ?? LastTrade,
                    SecondStudyStartingValue = lastMinute?.SecondStudyValue ?? LastTrade,
                };
            }
        }


    }

}