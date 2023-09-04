using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "settledUnsettled")]
    public class SettledUnsettled
    {

        [XmlElement(ElementName = "currentBp")]
        public double CurrentBp { get; set; }

        [XmlElement(ElementName = "currentNetBp")]
        public double CurrentNetBp { get; set; }

        [XmlElement(ElementName = "currentOor")]
        public double CurrentOor { get; set; }

        [XmlElement(ElementName = "currentOrderImpact")]
        public double CurrentOrderImpact { get; set; }

        [XmlElement(ElementName = "netBp")]
        public DateTime NetBp { get; set; }
    }


}
