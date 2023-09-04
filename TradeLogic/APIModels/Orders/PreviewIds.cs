using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "PreviewIds")]
    public class PreviewIds
    {

        [XmlElement(ElementName = "previewId")]
        public int PreviewId { get; set; }
    }


}
