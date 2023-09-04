using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.APIModels.Orders
{
    public class Event
    {
        public string name { get; set; }
        public long dateTime { get; set; }
        public int orderNumber { get; set; }
        public List<Instrument> instrument { get; set; }
    }
}
