using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "Order")]
    public class Order
    {

        [XmlElement(ElementName = "orderId")]
        public int OrderId { get; set; }

        [XmlElement(ElementName = "details")]
        public string Details { get; set; }

        [XmlElement(ElementName = "orderType")]
        public string OrderType { get; set; }

        [XmlElement(ElementName = "OrderDetail")]
        public List<OrderDetail> OrderDetail { get; set; }

        [XmlElement(ElementName = "Events")]
        public List<Event> events { get; set; }

    }
}
