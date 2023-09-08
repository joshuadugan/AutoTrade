using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.APIModels.Orders;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Orders
{
    public partial class PreviewOrderResponse : IResource, IRequest<PreviewOrderResponse.RequestBody>, IBelongToOrderService
    {
        public static Dictionary<string, string> RequestParameters(string accountIdKey)
        {
            var par = new Dictionary<string, string>
            {
                { "accountIdKey", accountIdKey },
            };
            return par;
        }

        private const string ResourceNameFormatString = "v1/accounts/{accountIdKey}/orders/preview";
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

            public RequestBody(
                OrderTypes orderType,
                string clientOrderId,
                string symbol,
                int orderedQuantity,
                decimal limitPrice,
                decimal stopPrice,
                OrderAction orderAction,
                PriceType priceType = PriceType.LIMIT,
                MarketSession marketSession = MarketSession.REGULAR,
                OrderTerm orderTerm = OrderTerm.GOOD_FOR_DAY,
                OffsetTypes offsetType = OffsetTypes.TRAILING_STOP_CNST,
                decimal offsetValue = .50m)
            {
                OrderType = orderType;
                ClientOrderId = clientOrderId;
                Order.Add(new OrderDetail()
                {
                    LimitPrice = limitPrice,
                    StopPrice = stopPrice,
                    MarketSession = marketSession.ToString(),
                    OrderTerm = orderTerm.ToString(),
                    PriceType = priceType.ToString(),
                    OffsetType = offsetType.ToString(),
                    OffsetValue = offsetValue,
                    Instrument = new List<Instrument>()
                    {
                        new Instrument()
                        {
                             OrderAction = orderAction.ToString(),
                             OrderedQuantity=orderedQuantity,
                              Product = new Product()
                              {
                                   Symbol=symbol
                              }

                        }
                    }
                });
            }
            public enum OrderAction
            {
                BUY, SELL, BUY_TO_COVER, SELL_SHORT, BUY_OPEN, BUY_CLOSE, SELL_OPEN, SELL_CLOSE, EXCHANGE
            }
            public enum PriceType
            {
                MARKET, LIMIT, STOP, STOP_LIMIT, TRAILING_STOP_CNST_BY_LOWER_TRIGGER, UPPER_TRIGGER_BY_TRAILING_STOP_CNST, TRAILING_STOP_PRCT_BY_LOWER_TRIGGER, UPPER_TRIGGER_BY_TRAILING_STOP_PRCT, TRAILING_STOP_CNST, TRAILING_STOP_PRCT, HIDDEN_STOP, HIDDEN_STOP_BY_LOWER_TRIGGER, UPPER_TRIGGER_BY_HIDDEN_STOP, NET_DEBIT, NET_CREDIT, NET_EVEN, MARKET_ON_OPEN, MARKET_ON_CLOSE, LIMIT_ON_OPEN, LIMIT_ON_CLOSE
            }
            public enum MarketSession
            {
                REGULAR, EXTENDED
            }
            public enum OrderTerm
            {
                GOOD_UNTIL_CANCEL, GOOD_FOR_DAY, GOOD_TILL_DATE, IMMEDIATE_OR_CANCEL, FILL_OR_KILL
            }
            public enum OrderTypes
            {
                EQ, OPTN, SPREADS, BUY_WRITES, BUTTERFLY, IRON_BUTTERFLY, CONDOR, IRON_CONDOR, MF, MMF
            }

            public enum OffsetTypes
            {
                TRAILING_STOP_CNST, TRAILING_STOP_PRCT
            }
            public OrderTypes OrderType { get; set; }
            public string ClientOrderId { get; set; }
            public List<OrderDetail> Order { get; set; } = new();


        }


    }

    [XmlRoot(ElementName = "PreviewOrderResponse")]
    public partial class PreviewOrderResponse
    {

        [XmlElement(ElementName = "clientOrderId")]
        public string ClientOrderId { get; set; }

        [XmlElement(ElementName = "orderType")]
        public string OrderType { get; set; }

        [XmlElement(ElementName = "totalOrderValue")]
        public DateTime TotalOrderValue { get; set; }

        [XmlElement(ElementName = "Order")]
        public List<OrderDetail> Order { get; set; }

        [XmlElement(ElementName = "PreviewIds")]
        public List<PreviewIds> PreviewIds { get; set; }

        [XmlElement(ElementName = "previewTime")]
        public double PreviewTime { get; set; }

        [XmlElement(ElementName = "dstFlag")]
        public bool DstFlag { get; set; }

        [XmlElement(ElementName = "accountId")]
        public int AccountId { get; set; }

        [XmlElement(ElementName = "optionLevelCd")]
        public int OptionLevelCd { get; set; }

        [XmlElement(ElementName = "marginLevelCd")]
        public string MarginLevelCd { get; set; }

        [XmlElement(ElementName = "Disclosure")]
        public Disclosure Disclosure { get; set; }

        [XmlElement(ElementName = "cashBpDetails")]
        public CashBpDetails CashBpDetails { get; set; }
    }


}
