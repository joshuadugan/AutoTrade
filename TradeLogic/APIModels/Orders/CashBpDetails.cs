using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "cashBpDetails")]
    public class CashBpDetails
    {

        [XmlElement(ElementName = "settled")]
        public Settled Settled { get; set; }

        [XmlElement(ElementName = "settledUnsettled")]
        public SettledUnsettled SettledUnsettled { get; set; }
    }


}
