using AutoTradeMobile.DataClasses;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TradeLogic;
using TradeLogic.APIModels.Orders;
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    public partial class TradeApp : ObservableObject
    {

        public bool UseSandBox { get; }
        public AuthDataContainer AuthData { get; } = new();
        public PersistedData StoredData { get; } = new();
        public SymbolData SymbolData { get; } = new();

        [ObservableProperty]
        ObservableCollection<MarketOrder> orders = new();

        [ObservableProperty]
        ObservableCollection<Account> accounts;

        TradeLogic.Trader _trader;
        public TradeLogic.Trader TradeAPI
        {
            get
            {
                if (_trader == null)
                {
                    _trader = new TradeLogic.Trader(AuthData.AuthKey, AuthData.AuthSecret, UseSandBox);
                }
                return _trader;
            }
            set
            {
                _trader = value;
            }
        }
        public AccessToken AccessToken { get; set; }
        public List<string> CurrentSymbolList { get; } = new();
        public string AccountIdKey { get; private set; }

        [ObservableProperty]
        int totalRequests;

        [ObservableProperty]
        TimeSpan lastQuoteResponseTime;

        [ObservableProperty]
        TimeSpan lastOrderResponseTime;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        string tradingError;

        public bool HasError
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TradingError);
            }
        }


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

        internal async void LoadOrdersAsync(bool replayLastSession)
        {
            if (replayLastSession) return;
            string symbol = CurrentSymbolList.First();
            OrdersListResponse OrderResult = await TradeAPI.GetOrdersAsync(AccessToken, AccountIdKey, symbol);
            if (OrderResult != null)
            {
                Orders = OrderResult.Order.Select(order => new MarketOrder(order)).ToObservableCollection();

            }
        }

        internal async void LoadPortfolioAsync(bool replayLastSession)
        {
            if (replayLastSession)
            {
                //process items in the que to simulate getting new portfolio data
                if (CurrentPositionQueue.Count > 0)
                {
                    var pos = CurrentPositionQueue.Dequeue();
                    if (SymbolData.CurrentPosition == null)
                    {
                        SymbolData.CurrentPosition = new CurrentPosition(pos);
                    }
                    else
                    {
                        SymbolData.CurrentPosition.MergeNewOrder(pos);
                    }
                }
                return;
            }

            string symbol = CurrentSymbolList.First();
            var Portfolio = await TradeAPI.ViewPortfolioPerformanceAsync(AccessToken, AccountIdKey);
            var SymbolShares = Portfolio.AccountPortfolio.Position.FindAll(p => p.Product.Symbol.Equals(symbol));
            SymbolData.ProcessPortfolioResponseData(SymbolShares);
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


    }
}
