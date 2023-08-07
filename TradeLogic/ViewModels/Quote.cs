using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.ViewModels
{
    public class Quote
    {
        public string Symbol { get; set; } = string.Empty;
        public double Price { get; set; }
        public double MovingAverage { get; set; }
        public double FiftyTwoWeekHigh { get; set; }
        public double FiftyTwoWeekLow { get; set; }
        public DateTime Date { get; set; }
        public double AnnualDividendPercent { get; set; }
    }

}
