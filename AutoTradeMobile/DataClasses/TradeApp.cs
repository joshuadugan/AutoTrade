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

        public TradeApp()
        {
            orders.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TodayProfit));
                OnPropertyChanged(nameof(TodayProfitColor));
            };

        }

        public Color TodayProfitColor
        {
            get
            {
                return TodayProfit >= 0 ? Colors.LightGreen : Colors.Red;
            }
        }

        public double TodayProfit
        {
            get
            {
                return TodayOrderSells - TodayOrderBuys;
            }
        }

        public double TodayOrderBuys
        {
            get
            {
                return Orders.Where(o => o.OrderAction == MarketOrder.OrderActions.BUY).Sum(o => o.OrderValue);
            }
        }

        public double TodayOrderSells
        {
            get
            {
                return Orders.Where(o => o.OrderAction == MarketOrder.OrderActions.SELL).Sum(o => o.OrderValue);
            }
        }



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
        }
        public AccessToken AccessToken { get; set; }
        public string Symbol { get; set; }
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

        internal async void StartTradingSymbolAsync(string symbol)
        {
            //Debug.Assert(symbol != null);

            //lookup the symbol to see if its valid
            if (await TradeAPI.ValidateSymbolAsync(symbol))
            {
                Symbol = symbol;
                AccountIdKey = StoredData.LastAccount.AccountIdKey;

                StartTrading();
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
            if (SimulateOrders) return;
            OrdersListResponse OrderResult = await TradeAPI.GetOrdersAsync(AccessToken, AccountIdKey, Symbol);
            if (OrderResult != null)
            {
                Orders = OrderResult.Order.Select(order => new MarketOrder(order)).ToObservableCollection();

            }
        }

        internal async void LoadPortfolioAsync()
        {
            if (SimulateOrders)
            {
                //process items in the que to simulate getting new portfolio data
                if (CurrentPositionQueue.Count > 0)
                {
                    var pos = CurrentPositionQueue.Dequeue();
                    SymbolData.CurrentPosition.MergeNewOrder(pos);
                    DequeOrderProcessingQueue();
                }
                return;
            }

            var Portfolio = await TradeAPI.ViewPortfolioPerformanceAsync(AccessToken, AccountIdKey);
            var SymbolShares = Portfolio.AccountPortfolio.Position.FindAll(p => p.Product.Symbol.Equals(Symbol));
            SymbolData.ProcessPortfolioResponseData(SymbolShares);
        }

        internal void StartTrading()
        {
            StartTickerTimer();
            StartOrderTimer();
        }

        internal void StopTrading()
        {
            TickerTimer.Dispose();
        }


    }
}
