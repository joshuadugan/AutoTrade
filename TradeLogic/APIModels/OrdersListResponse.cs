using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Orders
{


    [XmlRoot(ElementName = "OrdersResponse")]
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

        [XmlElement(ElementName = "Order")]
        public List<Order> Order { get; set; }

    }


}
