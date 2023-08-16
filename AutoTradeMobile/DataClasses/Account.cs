using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile.DataClasses
{
    public partial class Account: ObservableObject
    {
        public Account(TradeLogic.APIModels.Accounts.Account a)
        {
            AccountId = a.AccountId;
            AccountIdKey = a.AccountIdKey;
            AccountName = a.AccountName;

        }

        [ObservableProperty]
        int accountId;

        [ObservableProperty]
        string accountIdKey;

        [ObservableProperty]
        string accountName;

    }
}
