using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.APIModels.Accounts
{

    public partial class AccountListResponse : IResource, IBelongToAccountService
    {

        private const string ResourceNameFormatString = "/v1/accounts/list";

        public string GetResourceName()
        {
            return ResourceNameFormatString;
        }

    }

    [XmlRoot(ElementName = "Account")]
    public class Account
    {

        [XmlElement(ElementName = "accountId")]
        public int AccountId { get; set; }

        [XmlElement(ElementName = "accountIdKey")]
        public string AccountIdKey { get; set; }

        [XmlElement(ElementName = "accountMode")]
        public string AccountMode { get; set; }

        [XmlElement(ElementName = "accountDesc")]
        public string AccountDesc { get; set; }

        [XmlElement(ElementName = "accountName")]
        public string AccountName { get; set; }

        [XmlElement(ElementName = "accountType")]
        public string AccountType { get; set; }

        [XmlElement(ElementName = "institutionType")]
        public string InstitutionType { get; set; }

        [XmlElement(ElementName = "accountStatus")]
        public string AccountStatus { get; set; }

        [XmlElement(ElementName = "closedDate")]
        public int ClosedDate { get; set; }

        [XmlElement(ElementName = "shareWorksAccount")]
        public bool ShareWorksAccount { get; set; }

        [XmlElement(ElementName = "fcManagedMssbClosedAccount")]
        public bool FcManagedMssbClosedAccount { get; set; }
    }

    [XmlRoot(ElementName = "Accounts")]
    public class Accounts
    {

        [XmlElement(ElementName = "Account")]
        public List<Account> Account { get; set; }
    }

    [XmlRoot(ElementName = "AccountListResponse")]
    public partial class AccountListResponse
    {

        [XmlElement(ElementName = "Accounts")]
        public Accounts Accounts { get; set; }
    }



}
