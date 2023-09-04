using System.Xml.Serialization;

namespace TradeLogic.APIModels.Orders
{
    [XmlRoot(ElementName = "Instrument")]
    public class Instrument
    {

        [XmlElement(ElementName = "Product")]
        public Product Product { get; set; }

        [XmlElement(ElementName = "symbolDescription")]
        public string SymbolDescription { get; set; }

        [XmlElement(ElementName = "orderAction")]
        public string OrderAction { get; set; }

        [XmlElement(ElementName = "quantityType")]
        public string QuantityType { get; set; }

        [XmlElement(ElementName = "orderedQuantity")]
        public int OrderedQuantity { get; set; }

        [XmlElement(ElementName = "filledQuantity")]
        public decimal FilledQuantity { get; set; }

        [XmlElement(ElementName = "estimatedCommission")]
        public decimal EstimatedCommission { get; set; }

        [XmlElement(ElementName = "estimatedFees")]
        public decimal EstimatedFees { get; set; }

        [XmlElement(ElementName = "averageExecutionPrice")]
        public decimal AverageExecutionPrice { get; set; }
    }


}
