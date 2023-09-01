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
            public decimal Ask { get; set; }
            public decimal Bid { get; set; }
            public decimal LastTrade { get; set; }
            public decimal Volume { get; set; }

            public Minute ToMinute(Minute lastMinute)
            {
                return new Minute(this);
            }
        }


    }

}