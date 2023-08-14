namespace AutoTradeMobile
{
    public partial class SymbolData
    {
        public class Tick
        {
            public long Time { get; set; }
            public string MinuteTime
            {
                get
                {
                    return DateTime.FromFileTimeUtc(Time).ToString("HH:mm");
                }
            }
            public double Ask { get; set; }
            public double Bid { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double LastTrade { get; set; }

            public Minute ToMinute(double close)
            {
                return new Minute()
                {
                    TradeMinute = MinuteTime,
                    Open = LastTrade,
                    High = LastTrade,
                    Low = LastTrade,
                    Close = LastTrade,
                    MinuteColor = LastTrade > close ? Colors.Green : Colors.Red
                };
            }
        }


    }

}