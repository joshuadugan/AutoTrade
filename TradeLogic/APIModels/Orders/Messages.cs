using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "messages")]
    public class Messages
    {

        [XmlElement(ElementName = "Message")]
        public List<Message> Message { get; set; }
    }


}
