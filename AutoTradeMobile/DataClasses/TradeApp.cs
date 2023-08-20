using AutoTradeMobile.DataClasses;
using CommunityToolkit.Maui.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.APIModels.Orders;
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        internal async Task<bool> StartAuthProcessAsync()
        {
            bool OpenWindow = false;

            string AuthorizationUrl = await TradeAPI.GetAuthorizationUrlAsync();

            OpenWindow = await Browser.Default.OpenAsync(new Uri(AuthorizationUrl), BrowserLaunchMode.SystemPreferred);

            return OpenWindow;
        }

        internal async Task<bool> VerifyCodeAsync(string code)
        {
            AccessToken = await TradeAPI.GetAccessToken(code);
            if (AccessToken != null)
            {
                return true;
            }
            throw new Exception($"Unable to obtain access token");
        }

        internal async void StartTradingSymbolAsync(string symbol, bool replayLastSession)
        {
            //Debug.Assert(symbol != null); Debug.Assert(accountId != null);

            //lookup the symbol to see if its valid
            if (await TradeAPI.ValidateSymbolAsync(symbol))
            {
                if (!CurrentSymbolList.Contains(symbol))
                {
                    CurrentSymbolList.Add(symbol.ToUpper());
                }
                AccountIdKey = StoredData.LastAccount.AccountIdKey;

                StartTrading(replayLastSession);
            }
        }

        internal async void LoadAccountsAsync()
        {
            var result = await TradeAPI.ListAccountsAsync(AccessToken);
            Accounts = result.Accounts.Account.Select(a => new Account(a)).ToObservableCollection();
            Trace.WriteLine($"{Accounts.Count} Accounts");
        }

        internal async void LoadOrdersAsync()
        {
            string symbol = CurrentSymbolList.First();
            OrdersListResponse OrderResult = await TradeAPI.GetOrdersAsync(AccessToken, AccountIdKey, symbol);
            Orders = OrderResult.Order.Select(order => new MarketOrder(order)).ToObservableCollection();
        }

        internal SymbolData GetSymbolData(string symbol)
        {
            return Symbols.GetOrAdd(symbol.ToUpper(), key => { return new SymbolData(); });
        }

        internal void StartTrading(bool ReplayLastSession)
        {
            StartTickerTimer(ReplayLastSession);
            StartOrderTimer(ReplayLastSession);
        }

        internal void StopTrading()
        {
            TickerTimer.Dispose();
        }

        internal void PlaceOrder(Order order)
        {

        }
    }
}
