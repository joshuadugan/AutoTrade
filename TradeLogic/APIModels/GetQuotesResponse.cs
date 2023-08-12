﻿using System;
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
        public double Ask { get; set; }

        [XmlElement(ElementName = "askSize")]
        public Int64 AskSize { get; set; }

        [XmlElement(ElementName = "askTime")]
        public string AskTime { get; set; }

        [XmlElement(ElementName = "bid")]
        public double Bid { get; set; }

        [XmlElement(ElementName = "bidExchange")]
        public string BidExchange { get; set; }

        [XmlElement(ElementName = "bidSize")]
        public Int64 BidSize { get; set; }

        [XmlElement(ElementName = "bidTime")]
        public string BidTime { get; set; }

        [XmlElement(ElementName = "changeClose")]
        public double ChangeClose { get; set; }

        [XmlElement(ElementName = "changeClosePercentage")]
        public double ChangeClosePercentage { get; set; }

        [XmlElement(ElementName = "companyName")]
        public string CompanyName { get; set; }

        [XmlElement(ElementName = "daysToExpiration")]
        public Int64 DaysToExpiration { get; set; }

        [XmlElement(ElementName = "dirLast")]
        public string DirLast { get; set; }

        [XmlElement(ElementName = "dividend")]
        public double Dividend { get; set; }

        [XmlElement(ElementName = "eps")]
        public double Eps { get; set; }

        [XmlElement(ElementName = "estEarnings")]
        public double EstEarnings { get; set; }

        [XmlElement(ElementName = "exDividendDate")]
        public Int64 ExDividendDate { get; set; }

        [XmlElement(ElementName = "high")]
        public double High { get; set; }

        [XmlElement(ElementName = "high52")]
        public double High52 { get; set; }

        [XmlElement(ElementName = "lastTrade")]
        public double LastTrade { get; set; }

        [XmlElement(ElementName = "low")]
        public double Low { get; set; }

        [XmlElement(ElementName = "low52")]
        public double Low52 { get; set; }

        [XmlElement(ElementName = "open")]
        public double Open { get; set; }

        [XmlElement(ElementName = "openInterest")]
        public Int64 OpenInterest { get; set; }

        [XmlElement(ElementName = "optionStyle")]
        public string OptionStyle { get; set; }

        [XmlElement(ElementName = "previousClose")]
        public double PreviousClose { get; set; }

        [XmlElement(ElementName = "previousDayVolume")]
        public Int64 PreviousDayVolume { get; set; }

        [XmlElement(ElementName = "primaryExchange")]
        public string PrimaryExchange { get; set; }

        [XmlElement(ElementName = "symbolDescription")]
        public string SymbolDescription { get; set; }

        [XmlElement(ElementName = "totalVolume")]
        public Int64 TotalVolume { get; set; }

        [XmlElement(ElementName = "upc")]
        public Int64 Upc { get; set; }

        [XmlElement(ElementName = "cashDeliverable")]
        public decimal CashDeliverable { get; set; }

        [XmlElement(ElementName = "marketCap")]
        public double MarketCap { get; set; }

        [XmlElement(ElementName = "sharesOutstanding")]
        public double SharesOutstanding { get; set; }

        [XmlElement(ElementName = "nextEarningDate")]
        public string NextEarningDate { get; set; }

        [XmlElement(ElementName = "beta")]
        public double Beta { get; set; }

        [XmlElement(ElementName = "yield")]
        public double Yield { get; set; }

        [XmlElement(ElementName = "declaredDividend")]
        public double DeclaredDividend { get; set; }

        [XmlElement(ElementName = "dividendPayableDate")]
        public Int64 DividendPayableDate { get; set; }

        [XmlElement(ElementName = "pe")]
        public double Pe { get; set; }

        [XmlElement(ElementName = "week52LowDate")]
        public Int64 Week52LowDate { get; set; }

        [XmlElement(ElementName = "week52HiDate")]
        public Int64 Week52HiDate { get; set; }

        [XmlElement(ElementName = "intrinsicValue")]
        public double IntrinsicValue { get; set; }

        [XmlElement(ElementName = "timePremium")]
        public double TimePremium { get; set; }

        [XmlElement(ElementName = "optionMultiplier")]
        public double OptionMultiplier { get; set; }

        [XmlElement(ElementName = "contractSize")]
        public double ContractSize { get; set; }

        [XmlElement(ElementName = "expirationDate")]
        public Int64 ExpirationDate { get; set; }

        [XmlElement(ElementName = "timeOfLastTrade")]
        public Int64 TimeOfLastTrade { get; set; }

        [XmlElement(ElementName = "averageVolume")]
        public Int64 AverageVolume { get; set; }
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
        public Int64 DateTimeUTC { get; set; }

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
        public double Ask { get; set; }

        [XmlElement(ElementName = "bid")]
        public double Bid { get; set; }

        [XmlElement(ElementName = "changeClose")]
        public double ChangeClose { get; set; }

        [XmlElement(ElementName = "changeClosePercentage")]
        public double ChangeClosePercentage { get; set; }

        [XmlElement(ElementName = "companyName")]
        public string CompanyName { get; set; }

        [XmlElement(ElementName = "high")]
        public double High { get; set; }

        [XmlElement(ElementName = "lastTrade")]
        public double LastTrade { get; set; }

        [XmlElement(ElementName = "low")]
        public double Low { get; set; }

        [XmlElement(ElementName = "totalVolume")]
        public Int64 TotalVolume { get; set; }
    }


}
