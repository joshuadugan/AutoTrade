using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "OrderDetail")]
    public class OrderDetail
    {

        [XmlElement(ElementName = "Instrument")]
        public List<Instrument> Instrument { get; set; }

        [XmlElement(ElementName = "netPrice")]
        public decimal NetPrice { get; set; }

        [XmlElement(ElementName = "netBid")]
        public decimal NetBid { get; set; }

        [XmlElement(ElementName = "netAsk")]
        public decimal NetAsk { get; set; }

        [XmlElement(ElementName = "gcd")]
        public decimal Gcd { get; set; }

        [XmlElement(ElementName = "ratio")]
        public string Ratio { get; set; }

        [XmlElement(ElementName = "placedTime")]
        public long PlacedTime { get; set; }

        [XmlElement(ElementName = "executedTime")]
        public long ExecutedTime { get; set; }

        [XmlElement(ElementName = "orderValue")]
        public decimal OrderValue { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "orderTerm")]
        public string OrderTerm { get; set; }

        [XmlElement(ElementName = "priceType")]
        public string PriceType { get; set; }

        [XmlElement(ElementName = "limitPrice")]
        public decimal LimitPrice { get; set; }

        [XmlElement(ElementName = "stopPrice")]
        public decimal StopPrice { get; set; }

        [XmlElement(ElementName = "marketSession")]
        public string MarketSession { get; set; }

        [XmlElement(ElementName = "replacesOrderId")]
        public int ReplacesOrderId { get; set; }

        [XmlElement(ElementName = "allOrNone")]
        public bool AllOrNone { get; set; }

        [XmlElement(ElementName = "replacedByOrderId")]
        public int ReplacedByOrderId { get; set; }

        [XmlElement(ElementName = "offsetType")]
        public string OffsetType { get; set; }

        [XmlElement(ElementName = "offsetValue")]
        public decimal OffsetValue { get; set; }
        public decimal PeakPrice { get; set; }
    }

}
