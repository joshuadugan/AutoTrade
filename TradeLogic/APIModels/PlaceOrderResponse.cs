using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Orders
{
    public class PlaceOrderResponse : IResource, IRequest<PlaceOrderResponse.RequestBody>, IBelongToOrderService
    {
        public static Dictionary<string, string> RequestParameters(string accountIdKey)
        {
            var par = new Dictionary<string, string>
            {
                { "accountIdKey", accountIdKey },
            };
            return par;
        }

        private const string ResourceNameFormatString = "v1/accounts/{accountIdKey}/orders/place";
        public string GetResourceName()
        {
            return ResourceNameFormatString;
        }

        public RequestBody RequestBodyData { get; set; }

        public RequestBody ToRequestBodyObject()
        {
            return RequestBodyData;
        }

        public class RequestBody
        {
            public RequestBody(PreviewOrderResponse previewResponse)
            {
                PreviewIds = previewResponse.PreviewIds;
                OrderType = previewResponse.OrderType;
                ClientOrderId = previewResponse.ClientOrderId;
                Order = previewResponse.Order;
            }

            public string OrderType { get; set; }
            public string ClientOrderId { get; set; }
            public List<PreviewIds> PreviewIds { get; set; }
            public List<OrderDetail> Order { get; set; } = new();

        }

    }
}
