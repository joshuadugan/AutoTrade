using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "Disclosure")]
    public class Disclosure
    {

        [XmlElement(ElementName = "ahDisclosureFlag")]
        public bool AhDisclosureFlag { get; set; }

        [XmlElement(ElementName = "aoDisclosureFlag")]
        public bool AoDisclosureFlag { get; set; }

        [XmlElement(ElementName = "conditionalDisclosureFlag")]
        public bool ConditionalDisclosureFlag { get; set; }

        [XmlElement(ElementName = "ehDisclosureFlag")]
        public bool EhDisclosureFlag { get; set; }
    }


}
