using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "productId")]
    public class ProductId
    {

        [XmlElement(ElementName = "symbol")]
        public string Symbol { get; set; }

        [XmlElement(ElementName = "typeCode")]
        public string TypeCode { get; set; }
    }


}
