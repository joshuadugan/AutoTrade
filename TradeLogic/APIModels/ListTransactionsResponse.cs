using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Accounts
{
    public partial class ListTransactionsResponse : IResource, IBelongToAccountService
    {
        public static Dictionary<string, string> RequestParameters(string accountIdKey)
        {
            var par = new Dictionary<string, string>
            {
                { "accountIdKey", accountIdKey },
            };
            return par;
        }

        private const string ResourceNameFormatString = "/v1/accounts/{accountIdKey}/transactions";
        public string GetResourceName()
        {
            return ResourceNameFormatString;
        }

    }

    [XmlRoot(ElementName = "product")]
    public class Product
    {

        [XmlElement(ElementName = "callPut")]
        public string CallPut { get; set; }

        [XmlElement(ElementName = "expiryDay")]
        public long ExpiryDay { get; set; }

        [XmlElement(ElementName = "expiryMonth")]
        public long ExpiryMonth { get; set; }

        [XmlElement(ElementName = "expiryYear")]
        public long ExpiryYear { get; set; }

        [XmlElement(ElementName = "securityType")]
        public string SecurityType { get; set; }

        [XmlElement(ElementName = "strikePrice")]
        public double StrikePrice { get; set; }

        [XmlElement(ElementName = "symbol")]
        public string Symbol { get; set; }
    }

    [XmlRoot(ElementName = "brokerage")]
    public class Brokerage
    {

        [XmlElement(ElementName = "product")]
        public Product Product { get; set; }

        [XmlElement(ElementName = "quantity")]
        public decimal Quantity { get; set; }

        [XmlElement(ElementName = "price")]
        public decimal Price { get; set; }

        [XmlElement(ElementName = "settlementCurrency")]
        public string SettlementCurrency { get; set; }

        [XmlElement(ElementName = "paymentCurrency")]
        public string PaymentCurrency { get; set; }

        [XmlElement(ElementName = "fee")]
        public decimal Fee { get; set; }

        [XmlElement(ElementName = "displaySymbol")]
        public string DisplaySymbol { get; set; }

        [XmlElement(ElementName = "settlementDate")]
        public long SettlementDate { get; set; }
    }

    [XmlRoot(ElementName = "Transaction")]
    public class Transaction
    {

        [XmlElement(ElementName = "transactionId")]
        public double TransactionId { get; set; }

        [XmlElement(ElementName = "accountId")]
        public int AccountId { get; set; }

        [XmlElement(ElementName = "transactionDate")]
        public double TransactionDate { get; set; }

        [XmlElement(ElementName = "postDate")]
        public long PostDate { get; set; }

        [XmlElement(ElementName = "amount")]
        public decimal Amount { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "transactionType")]
        public string TransactionType { get; set; }

        [XmlElement(ElementName = "memo")]
        public string Memo { get; set; }

        [XmlElement(ElementName = "imageFlag")]
        public bool ImageFlag { get; set; }

        [XmlElement(ElementName = "instType")]
        public string InstType { get; set; }

        [XmlElement(ElementName = "storeId")]
        public int StoreId { get; set; }

        [XmlElement(ElementName = "brokerage")]
        public Brokerage Brokerage { get; set; }

        [XmlElement(ElementName = "detailsURI")]
        public string DetailsURI { get; set; }
    }

    [XmlRoot(ElementName = "TransactionListResponse")]
    public partial class ListTransactionsResponse
    {

        [XmlElement(ElementName = "Transaction")]
        public List<Transaction> Transaction { get; set; }

        [XmlElement(ElementName = "next")]
        public string Next { get; set; }

        [XmlElement(ElementName = "marker")]
        public string Marker { get; set; }

        [XmlElement(ElementName = "pageMarkers")]
        public string PageMarkers { get; set; }

        [XmlElement(ElementName = "moreTransactions")]
        public bool MoreTransactions { get; set; }

        [XmlElement(ElementName = "transactionCount")]
        public int TransactionCount { get; set; }

        [XmlElement(ElementName = "totalCount")]
        public int TotalCount { get; set; }
    }


}
