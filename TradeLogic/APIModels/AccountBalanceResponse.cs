using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Accounts
{
    public partial class AccountBalanceResponse : IResource, IBelongToAccountService
    {
        public static Dictionary<string, string> RequestParameters(string accountIdKey, string instType = "BROKERAGE", bool realTimeNAV = true)
        {
            var par = new Dictionary<string, string>
            {
                { "accountIdKey", accountIdKey },
                { "instType", instType },
                { "realTimeNAV", realTimeNAV.ToString() }
            };
            return par;
        }

        private const string ResourceNameFormatString = "/v1/accounts/{accountIdKey}/balance?instType={instType}&realTimeNAV={realTimeNAV}";
        public string GetResourceName()
        {
            return ResourceNameFormatString;
        }

    }

    [XmlRoot(ElementName = "Cash")]
    public class Cash
    {

        [XmlElement(ElementName = "fundsForOpenOrdersCash")]
        public double FundsForOpenOrdersCash { get; set; }

        [XmlElement(ElementName = "moneyMktBalance")]
        public double MoneyMktBalance { get; set; }
    }

    [XmlRoot(ElementName = "Computed")]
    public class Computed
    {

        [XmlElement(ElementName = "cashAvailableForInvestment")]
        public double CashAvailableForInvestment { get; set; }

        [XmlElement(ElementName = "cashAvailableForWithdrawal")]
        public double CashAvailableForWithdrawal { get; set; }

        [XmlElement(ElementName = "netCash")]
        public double NetCash { get; set; }

        [XmlElement(ElementName = "cashBalance")]
        public double CashBalance { get; set; }

        [XmlElement(ElementName = "settledCashForInvestment")]
        public double SettledCashForInvestment { get; set; }

        [XmlElement(ElementName = "unSettledCashForInvestment")]
        public double UnSettledCashForInvestment { get; set; }

        [XmlElement(ElementName = "fundsWithheldFromPurchasePower")]
        public double FundsWithheldFromPurchasePower { get; set; }

        [XmlElement(ElementName = "fundsWithheldFromWithdrawal")]
        public double FundsWithheldFromWithdrawal { get; set; }

        [XmlElement(ElementName = "marginBuyingPower")]
        public double MarginBuyingPower { get; set; }

        [XmlElement(ElementName = "cashBuyingPower")]
        public double CashBuyingPower { get; set; }

        [XmlElement(ElementName = "dtMarginBuyingPower")]
        public double DtMarginBuyingPower { get; set; }

        [XmlElement(ElementName = "dtCashBuyingPower")]
        public double DtCashBuyingPower { get; set; }

        [XmlElement(ElementName = "shortAdjustBalance")]
        public double ShortAdjustBalance { get; set; }

        [XmlElement(ElementName = "regtEquity")]
        public double RegtEquity { get; set; }

        [XmlElement(ElementName = "regtEquityPercent")]
        public double RegtEquityPercent { get; set; }

        [XmlElement(ElementName = "accountBalance")]
        public double AccountBalance { get; set; }
    }

    [XmlRoot(ElementName = "Margin")]
    public class Margin
    {

        [XmlElement(ElementName = "dtCashOpenOrderReserve")]
        public double DtCashOpenOrderReserve { get; set; }

        [XmlElement(ElementName = "dtMarginOpenOrderReserve")]
        public double DtMarginOpenOrderReserve { get; set; }
    }

    [XmlRoot(ElementName = "BalanceResponse")]
    public partial class AccountBalanceResponse
    {

        [XmlElement(ElementName = "accountId")]
        public int AccountId { get; set; }

        [XmlElement(ElementName = "accountType")]
        public string AccountType { get; set; }

        [XmlElement(ElementName = "optionLevel")]
        public string OptionLevel { get; set; }

        [XmlElement(ElementName = "accountDescription")]
        public string AccountDescription { get; set; }

        [XmlElement(ElementName = "quoteMode")]
        public int QuoteMode { get; set; }

        [XmlElement(ElementName = "dayTraderStatus")]
        public string DayTraderStatus { get; set; }

        [XmlElement(ElementName = "accountMode")]
        public string AccountMode { get; set; }

        [XmlElement(ElementName = "Cash")]
        public Cash Cash { get; set; }

        [XmlElement(ElementName = "Computed")]
        public Computed Computed { get; set; }

        [XmlElement(ElementName = "Margin")]
        public Margin Margin { get; set; }
    }



}
