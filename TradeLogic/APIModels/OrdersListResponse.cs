using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Orders
{
    public partial class OrdersListResponse : IResource, IBelongToOrderService
    {
        public static Dictionary<string, string> RequestParameters(string accountIdKey, string symbol, DateTime fromDate = default(DateTime), DateTime toDate = default(DateTime))
        {
            if (fromDate.Equals(DateTime.MinValue)) { fromDate = DateTime.Today; }
            if (toDate.Equals(DateTime.MinValue)) { toDate = DateTime.Today; }

            var par = new Dictionary<string, string>
            {
                { "accountIdKey", accountIdKey },
                { "symbol", symbol },
                { "fromDate", fromDate.ToString("MMddyyyy") },
                { "toDate", toDate.ToString("MMddyyyy") }
            };
            return par;
        }

        private const string ResourceNameFormatString = "/v1/accounts/{accountIdKey}/orders?symbol={symbol}&fromDate={fromDate}&toDate={toDate}";
        public string GetResourceName()
        {
            return ResourceNameFormatString;
        }
    }


    [XmlRoot(ElementName = "productId")]
    public class ProductId
    {

        [XmlElement(ElementName = "symbol")]
        public string Symbol { get; set; }

        [XmlElement(ElementName = "typeCode")]
        public string TypeCode { get; set; }
    }

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
        public double StrikePrice { get; set; }

        [XmlElement(ElementName = "productId")]
        public ProductId ProductId { get; set; }
    }

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
        public double FilledQuantity { get; set; }

        [XmlElement(ElementName = "estimatedCommission")]
        public double EstimatedCommission { get; set; }

        [XmlElement(ElementName = "estimatedFees")]
        public double EstimatedFees { get; set; }

        [XmlElement(ElementName = "averageExecutionPrice")]
        public double AverageExecutionPrice { get; set; }
    }

    [XmlRoot(ElementName = "OrderDetail")]
    public class OrderDetail
    {

        [XmlElement(ElementName = "Instrument")]
        public List<Instrument> Instrument { get; set; }

        [XmlElement(ElementName = "netPrice")]
        public double NetPrice { get; set; }

        [XmlElement(ElementName = "netBid")]
        public double NetBid { get; set; }

        [XmlElement(ElementName = "netAsk")]
        public double NetAsk { get; set; }

        [XmlElement(ElementName = "gcd")]
        public double Gcd { get; set; }

        [XmlElement(ElementName = "ratio")]
        public string Ratio { get; set; }

        [XmlElement(ElementName = "placedTime")]
        public long PlacedTime { get; set; }

        [XmlElement(ElementName = "executedTime")]
        public long ExecutedTime { get; set; }

        [XmlElement(ElementName = "orderValue")]
        public double OrderValue { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "orderTerm")]
        public string OrderTerm { get; set; }

        [XmlElement(ElementName = "priceType")]
        public string PriceType { get; set; }

        [XmlElement(ElementName = "limitPrice")]
        public double LimitPrice { get; set; }

        [XmlElement(ElementName = "stopPrice")]
        public double StopPrice { get; set; }

        [XmlElement(ElementName = "marketSession")]
        public string MarketSession { get; set; }

        [XmlElement(ElementName = "replacesOrderId")]
        public int ReplacesOrderId { get; set; }

        [XmlElement(ElementName = "allOrNone")]
        public bool AllOrNone { get; set; }

        [XmlElement(ElementName = "replacedByOrderId")]
        public int ReplacedByOrderId { get; set; }
    }

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
        public OrderDetail OrderDetail { get; set; }
    }

    [XmlRoot(ElementName = "OrdersResponse")]
    public partial class OrdersListResponse
    {

        [XmlElement(ElementName = "Order")]
        public List<Order> Order { get; set; }
    }


}
