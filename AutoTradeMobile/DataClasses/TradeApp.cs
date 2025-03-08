using AutoTradeMobile.DataClasses;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Animations;
using Microsoft.VisualBasic;
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
using System.Text.Json;
using System.Threading.Tasks;
using TradeLogic;
using TradeLogic.APIModels.Orders;
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    public partial class TradeApp : ObservableObject
    {
        public bool UseSandBox { get; }
        public static AuthDataContainer AuthData { get; } = new();
        public static PersistedData Settings { get; } = new();
        public static SymbolData Symbol { get; } = new();
        public static OrderData OrderState { get; } = new();

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
        public string SymbolName { get; set; }
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
                SymbolName = symbol;
                AccountIdKey = Settings.LastAccount.AccountIdKey;

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
            OrdersListResponse OrderResult = await TradeAPI.GetOrdersAsync(AccessToken, AccountIdKey, SymbolName);
            if (OrderResult != null)
            {
                OrderState.Orders = OrderResult.Order.Select(order => new MarketOrder(order)).ToObservableCollection();
            }
        }

        internal async void PlaceOrder(PreviewOrderResponse.RequestBody thisOrder)
        {
            PreviewOrderResponse PreviewResponse = await TradeAPI.PreviewOrder(AccessToken, AccountIdKey, thisOrder);
            LogOrderResponse(PreviewResponse);

            var placeOrder = new PlaceOrderResponse.RequestBody(PreviewResponse);

            PlaceOrderResponse PlaceResponse = await TradeAPI.PlaceOrder(AccessToken, AccountIdKey, placeOrder);
            LogOrderResponse(PlaceResponse);
        }

        internal async void LoadPortfolioAsync()
        {
            var Portfolio = await TradeAPI.ViewPortfolioPerformanceAsync(AccessToken, AccountIdKey);
            var SymbolShares = Portfolio.AccountPortfolio.Position.FindAll(p => p.Product.Symbol.Equals(Symbol));
            Symbol.ProcessPortfolioResponseData(SymbolShares);
        }

        internal void StartTrading()
        {
            StartTickerTimer();
            StartOrderTimer();
        }

        internal void StopTrading()
        {
            TickerTimer.Dispose();
            OrderTimer.Dispose();
        }

        internal void ResetState()
        {
            OrderState.Orders.Clear();
            Symbol.ResetState();
            TotalRequests = 0;
            MarketOrder.SimulatedOrderId = 1;
            Symbol.CurrentPosition = new();
        }

        internal void LogOrderResponse(object ResponseObject)
        {
            LogToFile(JsonSerializer.Serialize(ResponseObject), "Orders.txt");
        }


    }
}
