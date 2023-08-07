using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.ViewModels
{
    public class Account
    {
        public uint Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Cash { get; set; }
        public double NetValue { get; set; }
        public IEnumerable<Position>? Positions { get; set; }
        public bool IsMargin { get; set; }
        public double MarginableSecurities { get; set; }
        public double MarginEquity { get; set; }
    }
}
