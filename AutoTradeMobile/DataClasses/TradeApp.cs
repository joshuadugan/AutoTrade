﻿using CommunityToolkit.Maui.Core.Extensions;
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
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        internal async Task<bool> StartAuthProcessAsync()
        {
            bool OpenWindow = false;

            string AuthorizationUrl = await Trader.GetAuthorizationUrlAsync();

            OpenWindow = await Browser.Default.OpenAsync(new Uri(AuthorizationUrl), BrowserLaunchMode.SystemPreferred);

            return OpenWindow;
        }

        internal async Task<bool> VerifyCodeAsync(string code)
        {
            AccessToken = await Trader.GetAccessToken(code);
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
            if (await Trader.ValidateSymbolAsync(symbol))
            {
                if (!CurrentSymbolList.Contains(symbol))
                {
                    CurrentSymbolList.Add(symbol.ToUpper());
                }
                AccountIdKey = StoredData.LastAccount.AccountIdKey;

                TickerTimer = new(RequestSymbolData, replayLastSession, 1000, 1000);
                
                OrdersTimer = new(RequestOrderData, null, 1000, 5000);
            }
        }

        internal async void LoadAccountsAsync()
        {
            var result = await Trader.ListAccountsAsync(AccessToken);
            Accounts = result.Accounts.Account.Select(a => new Account(a)).ToObservableCollection();
            Trace.WriteLine($"{Accounts.Count} Accounts");
        }

        internal SymbolData GetSymbolData(string symbol)
        {
            return Symbols.GetOrAdd(symbol.ToUpper(), key => { return new SymbolData(); });
        }

        internal void StartTrading()
        {
            TickerTimer = new(RequestSymbolData, null, 1000, 1000);
        }

        internal void StopTrading()
        {
            TickerTimer.Dispose();
        }

    }
}
