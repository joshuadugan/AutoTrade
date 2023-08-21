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
        public double  StrikePrice { get; set; }

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
        public double  Change { get; set; }

        [XmlElement(ElementName = "changePct")]
        public double  ChangePct { get; set; }

        [XmlElement(ElementName = "lastTrade")]
        public double  LastTrade { get; set; }

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
        public double PricePaid { get; set; }

        [XmlElement(ElementName = "commissions")]
        public double Commissions { get; set; }

        [XmlElement(ElementName = "otherFees")]
        public double OtherFees { get; set; }

        [XmlElement(ElementName = "quantity")]
        public double Quantity { get; set; }

        [XmlElement(ElementName = "positionIndicator")]
        public string PositionIndicator { get; set; }

        [XmlElement(ElementName = "positionType")]
        public string PositionType { get; set; }

        [XmlElement(ElementName = "daysGain")]
        public double  DaysGain { get; set; }

        [XmlElement(ElementName = "daysGainPct")]
        public double  DaysGainPct { get; set; }

        [XmlElement(ElementName = "marketValue")]
        public double  MarketValue { get; set; }

        [XmlElement(ElementName = "totalCost")]
        public double  TotalCost { get; set; }

        [XmlElement(ElementName = "totalGain")]
        public double  TotalGain { get; set; }

        [XmlElement(ElementName = "totalGainPct")]
        public double  TotalGainPct { get; set; }

        [XmlElement(ElementName = "pctOfPortfolio")]
        public double  PctOfPortfolio { get; set; }

        [XmlElement(ElementName = "costPerShare")]
        public double CostPerShare { get; set; }

        [XmlElement(ElementName = "todayCommissions")]
        public double  TodayCommissions { get; set; }

        [XmlElement(ElementName = "todayFees")]
        public double  TodayFees { get; set; }

        [XmlElement(ElementName = "todayPricePaid")]
        public double  TodayPricePaid { get; set; }

        [XmlElement(ElementName = "todayQuantity")]
        public double  TodayQuantity { get; set; }

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
