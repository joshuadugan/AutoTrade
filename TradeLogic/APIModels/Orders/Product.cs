using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "Product")]
    public class Product
    {

        [XmlElement(ElementName = "symbol")]
        public string Symbol { get; set; }

        [XmlElement(ElementName = "securityType")]
        public string SecurityType { get; set; }

        [XmlElement(ElementName = "callPut")]
        public string CallPut { get; set; }

        [XmlElement(ElementName = "expiryYear")]
        public int ExpiryYear { get; set; }

        [XmlElement(ElementName = "expiryMonth")]
        public int ExpiryMonth { get; set; }

        [XmlElement(ElementName = "expiryDay")]
        public int ExpiryDay { get; set; }

        [XmlElement(ElementName = "strikePrice")]
        public decimal StrikePrice { get; set; }

        [XmlElement(ElementName = "productId")]
        public ProductId ProductId { get; set; }
    }


}
