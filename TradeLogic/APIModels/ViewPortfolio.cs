using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Accounts.portfolio
{
    public partial class ViewPortfolio_PerformanceViewResponse : IResource, IBelongToAccountService
    {
        public static Dictionary<string, string> RequestParameters(string accountIdKey, bool totalsRequired = false)
        {
            var par = new Dictionary<string, string>
            {
                { "accountIdKey", accountIdKey },
                { "totalsRequired", totalsRequired.ToString() },
                { "view", "PERFORMANCE" }
            };
            return par;
        }

        private const string ResourceNameFormatString = "/v1/accounts/{accountIdKey}/portfolio?view={view}&totalsRequired={totalsRequired}";
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

        [XmlElement(ElementName = "expiryDay")]
        public int ExpiryDay { get; set; }

        [XmlElement(ElementName = "expiryMonth")]
        public int ExpiryMonth { get; set; }

        [XmlElement(ElementName = "expiryYear")]
        public int ExpiryYear { get; set; }

        [XmlElement(ElementName = "productId")]
        public ProductId ProductId { get; set; }

        [XmlElement(ElementName = "securityType")]
        public string SecurityType { get; set; }

        [XmlElement(ElementName = "strikePrice")]
        public decimal StrikePrice { get; set; }

        [XmlElement(ElementName = "symbol")]
        public string Symbol { get; set; }

        [XmlElement(ElementName = "callPut")]
        public string CallPut { get; set; }

        [XmlElement(ElementName = "securitySubType")]
        public string SecuritySubType { get; set; }
    }

    [XmlRoot(ElementName = "Performance")]
    public class Performance
    {

        [XmlElement(ElementName = "change")]
        public decimal Change { get; set; }

        [XmlElement(ElementName = "changePct")]
        public decimal ChangePct { get; set; }

        [XmlElement(ElementName = "lastTrade")]
        public decimal LastTrade { get; set; }

        [XmlElement(ElementName = "lastTradeTime")]
        public long LastTradeTime { get; set; }

        [XmlElement(ElementName = "quoteStatus")]
        public string QuoteStatus { get; set; }
    }

    [XmlRoot(ElementName = "Position")]
    public class Position
    {

        [XmlElement(ElementName = "positionId")]
        public double PositionId { get; set; }

        [XmlElement(ElementName = "Product")]
        public Product Product { get; set; }

        [XmlElement(ElementName = "symbolDescription")]
        public string SymbolDescription { get; set; }

        [XmlElement(ElementName = "dateAcquired")]
        public long DateAcquired { get; set; }

        [XmlElement(ElementName = "pricePaid")]
        public decimal PricePaid { get; set; }

        [XmlElement(ElementName = "commissions")]
        public decimal Commissions { get; set; }

        [XmlElement(ElementName = "otherFees")]
        public decimal OtherFees { get; set; }

        [XmlElement(ElementName = "quantity")]
        public decimal Quantity { get; set; }

        [XmlElement(ElementName = "positionIndicator")]
        public string PositionIndicator { get; set; }

        [XmlElement(ElementName = "positionType")]
        public string PositionType { get; set; }

        [XmlElement(ElementName = "daysGain")]
        public decimal DaysGain { get; set; }

        [XmlElement(ElementName = "daysGainPct")]
        public decimal DaysGainPct { get; set; }

        [XmlElement(ElementName = "marketValue")]
        public decimal MarketValue { get; set; }

        [XmlElement(ElementName = "totalCost")]
        public decimal TotalCost { get; set; }

        [XmlElement(ElementName = "totalGain")]
        public decimal TotalGain { get; set; }

        [XmlElement(ElementName = "totalGainPct")]
        public decimal TotalGainPct { get; set; }

        [XmlElement(ElementName = "pctOfPortfolio")]
        public decimal PctOfPortfolio { get; set; }

        [XmlElement(ElementName = "costPerShare")]
        public double CostPerShare { get; set; }

        [XmlElement(ElementName = "todayCommissions")]
        public decimal TodayCommissions { get; set; }

        [XmlElement(ElementName = "todayFees")]
        public decimal TodayFees { get; set; }

        [XmlElement(ElementName = "todayPricePaid")]
        public decimal TodayPricePaid { get; set; }

        [XmlElement(ElementName = "todayQuantity")]
        public decimal TodayQuantity { get; set; }

        [XmlElement(ElementName = "adjPrevClose")]
        public double AdjPrevClose { get; set; }

        [XmlElement(ElementName = "Performance")]
        public Performance Performance { get; set; }

        [XmlElement(ElementName = "lotsDetails")]
        public string LotsDetails { get; set; }

        [XmlElement(ElementName = "quoteDetails")]
        public string QuoteDetails { get; set; }

        [XmlElement(ElementName = "osiKey")]
        public string OsiKey { get; set; }
    }

    [XmlRoot(ElementName = "AccountPortfolio")]
    public class AccountPortfolio
    {

        [XmlElement(ElementName = "accountId")]
        public int AccountId { get; set; }

        [XmlElement(ElementName = "Position")]
        public List<Position> Position { get; set; }

        [XmlElement(ElementName = "totalPages")]
        public int TotalPages { get; set; }
    }

    [XmlRoot(ElementName = "PortfolioResponse")]
    public partial class ViewPortfolio_PerformanceViewResponse
    {

        [XmlElement(ElementName = "AccountPortfolio")]
        public AccountPortfolio AccountPortfolio { get; set; }
    }



}
