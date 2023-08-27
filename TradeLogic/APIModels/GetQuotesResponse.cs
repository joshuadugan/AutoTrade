using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Quotes
{
    public partial class GetQuotesResponse : IResource, IBelongToMarketService
    {
        public static Dictionary<string, string> RequestParameters(List<string> symbols, string detailFlag = "INTRADAY", bool skipMiniOptionsCheck = true)
        {
            var par = new Dictionary<string, string>
            {
                { "symbols", string.Join(",",symbols) },
                { "detailFlag", detailFlag },
                {"skipMiniOptionsCheck",skipMiniOptionsCheck.ToString()}
            };
            return par;
        }

        private const string ResourceNameFormatString = "/v1/market/quote/{symbols}?detailFlag={detailFlag}&skipMiniOptionsCheck={skipMiniOptionsCheck}";
        public string GetResourceName()
        {
            return ResourceNameFormatString;
        }
    }


    [XmlRoot(ElementName = "All")]
    public class All
    {

        [XmlElement(ElementName = "adjustedFlag")]
        public bool AdjustedFlag { get; set; }

        [XmlElement(ElementName = "ask")]
        public decimal Ask { get; set; }

        [XmlElement(ElementName = "askSize")]
        public long AskSize { get; set; }

        [XmlElement(ElementName = "askTime")]
        public string AskTime { get; set; }

        [XmlElement(ElementName = "bid")]
        public decimal Bid { get; set; }

        [XmlElement(ElementName = "bidExchange")]
        public string BidExchange { get; set; }

        [XmlElement(ElementName = "bidSize")]
        public long BidSize { get; set; }

        [XmlElement(ElementName = "bidTime")]
        public string BidTime { get; set; }

        [XmlElement(ElementName = "changeClose")]
        public decimal ChangeClose { get; set; }

        [XmlElement(ElementName = "changeClosePercentage")]
        public decimal ChangeClosePercentage { get; set; }

        [XmlElement(ElementName = "companyName")]
        public string CompanyName { get; set; }

        [XmlElement(ElementName = "daysToExpiration")]
        public long DaysToExpiration { get; set; }

        [XmlElement(ElementName = "dirLast")]
        public string DirLast { get; set; }

        [XmlElement(ElementName = "dividend")]
        public decimal Dividend { get; set; }

        [XmlElement(ElementName = "eps")]
        public decimal Eps { get; set; }

        [XmlElement(ElementName = "estEarnings")]
        public decimal EstEarnings { get; set; }

        [XmlElement(ElementName = "exDividendDate")]
        public long ExDividendDate { get; set; }

        [XmlElement(ElementName = "high")]
        public decimal High { get; set; }

        [XmlElement(ElementName = "high52")]
        public decimal High52 { get; set; }

        [XmlElement(ElementName = "lastTrade")]
        public decimal LastTrade { get; set; }

        [XmlElement(ElementName = "low")]
        public decimal Low { get; set; }

        [XmlElement(ElementName = "low52")]
        public decimal Low52 { get; set; }

        [XmlElement(ElementName = "open")]
        public decimal Open { get; set; }

        [XmlElement(ElementName = "openInterest")]
        public long OpenInterest { get; set; }

        [XmlElement(ElementName = "optionStyle")]
        public string OptionStyle { get; set; }

        [XmlElement(ElementName = "previousClose")]
        public decimal PreviousClose { get; set; }

        [XmlElement(ElementName = "previousDayVolume")]
        public long PreviousDayVolume { get; set; }

        [XmlElement(ElementName = "primaryExchange")]
        public string PrimaryExchange { get; set; }

        [XmlElement(ElementName = "symbolDescription")]
        public string SymbolDescription { get; set; }

        [XmlElement(ElementName = "totalVolume")]
        public long TotalVolume { get; set; }

        [XmlElement(ElementName = "upc")]
        public long Upc { get; set; }

        [XmlElement(ElementName = "cashDeliverable")]
        public decimal CashDeliverable { get; set; }

        [XmlElement(ElementName = "marketCap")]
        public decimal MarketCap { get; set; }

        [XmlElement(ElementName = "sharesOutstanding")]
        public decimal SharesOutstanding { get; set; }

        [XmlElement(ElementName = "nextEarningDate")]
        public string NextEarningDate { get; set; }

        [XmlElement(ElementName = "beta")]
        public decimal Beta { get; set; }

        [XmlElement(ElementName = "yield")]
        public decimal Yield { get; set; }

        [XmlElement(ElementName = "declaredDividend")]
        public decimal DeclaredDividend { get; set; }

        [XmlElement(ElementName = "dividendPayableDate")]
        public long DividendPayableDate { get; set; }

        [XmlElement(ElementName = "pe")]
        public decimal Pe { get; set; }

        [XmlElement(ElementName = "week52LowDate")]
        public long Week52LowDate { get; set; }

        [XmlElement(ElementName = "week52HiDate")]
        public long Week52HiDate { get; set; }

        [XmlElement(ElementName = "intrinsicValue")]
        public decimal IntrinsicValue { get; set; }

        [XmlElement(ElementName = "timePremium")]
        public decimal TimePremium { get; set; }

        [XmlElement(ElementName = "optionMultiplier")]
        public decimal OptionMultiplier { get; set; }

        [XmlElement(ElementName = "contractSize")]
        public decimal ContractSize { get; set; }

        [XmlElement(ElementName = "expirationDate")]
        public long ExpirationDate { get; set; }

        [XmlElement(ElementName = "timeOfLastTrade")]
        public long TimeOfLastTrade { get; set; }

        [XmlElement(ElementName = "averageVolume")]
        public long AverageVolume { get; set; }
    }

    [XmlRoot(ElementName = "Product")]
    public class Product
    {

        [XmlElement(ElementName = "securityType")]
        public string SecurityType { get; set; }

        [XmlElement(ElementName = "symbol")]
        public string Symbol { get; set; }
    }

    [XmlRoot(ElementName = "QuoteData")]
    public class QuoteData
    {

        [XmlElement(ElementName = "dateTime")]
        public string DateTime { get; set; }

        [XmlElement(ElementName = "dateTimeUTC")]
        public long DateTimeUTC { get; set; }

        [XmlElement(ElementName = "quoteStatus")]
        public string QuoteStatus { get; set; }

        [XmlElement(ElementName = "ahFlag")]
        public bool AhFlag { get; set; }

        [XmlElement(ElementName = "All")]
        public All All { get; set; }

        [XmlElement(ElementName = "Intraday")]
        public Intraday Intraday { get; set; }


        [XmlElement(ElementName = "Product")]
        public Product Product { get; set; }
    }

    [XmlRoot(ElementName = "QuoteResponse")]
    public partial class GetQuotesResponse
    {

        [XmlElement(ElementName = "QuoteData")]
        public QuoteData QuoteData { get; set; }
    }


    [XmlRoot(ElementName = "Intraday")]
    public class Intraday
    {

        [XmlElement(ElementName = "ask")]
        public decimal Ask { get; set; }

        [XmlElement(ElementName = "bid")]
        public decimal Bid { get; set; }

        [XmlElement(ElementName = "changeClose")]
        public decimal ChangeClose { get; set; }

        [XmlElement(ElementName = "changeClosePercentage")]
        public decimal ChangeClosePercentage { get; set; }

        [XmlElement(ElementName = "companyName")]
        public string CompanyName { get; set; }

        [XmlElement(ElementName = "high")]
        public decimal High { get; set; }

        [XmlElement(ElementName = "lastTrade")]
        public decimal LastTrade { get; set; }

        [XmlElement(ElementName = "low")]
        public decimal Low { get; set; }

        [XmlElement(ElementName = "totalVolume")]
        public long TotalVolume { get; set; }
    }


}
