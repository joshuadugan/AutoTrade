﻿using OAuth;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Principal;
using System.Xml.Serialization;
using TradeLogic.APIModels;
using TradeLogic.APIModels.Accounts;
using TradeLogic.APIModels.Accounts.portfolio;
using TradeLogic.APIModels.Orders;
using TradeLogic.APIModels.Quotes;
using TradeLogic.Authorization;
using TradeLogic.Authorization.interfaces;
using TradeLogic.Exceptions;
using TradeLogic.extensions;
using TradeLogic.ViewModels;

namespace TradeLogic
{
    public partial class Trader : TraderBase
    {

        private const string RequestTokenUrl = "https://api.etrade.com/oauth/request_token",
            AuthorizeUrl = "https://us.etrade.com/e/t/etws/authorize",
            AccessTokenUrl = "https://api.etrade.com/oauth/access_token",
            RenewAccessTokenUrl = "https://api.etrade.com/oauth/renew_access_token",
            RevokeAccessTokenURL = "https://api.etrade.com/oauth/revoke_access_token",
            DataUrl = "https://api.etrade.com",
            SandboxDataUrl = "https://apisb.etrade.com",
            Realm = "etrade.com";



        public Trader(string ConsumerKey, string ConsumerSecret, bool useSandbox) : base(ConsumerKey, ConsumerSecret)
        {
            _BaseURL = useSandbox ? SandboxDataUrl : DataUrl;
        }

        public async Task<AccountListResponse> ListAccountsAsync(AccessToken accessToken)
        {
            return await GetAsync<AccountListResponse>(null, accessToken);
        }

        public async Task<AccountBalanceResponse> GetAccountBalancesAsync(AccessToken accessToken, string accountIdKey)
        {
            return await GetAsync<AccountBalanceResponse>(AccountBalanceResponse.RequestParameters(accountIdKey), accessToken);
        }

        public async Task<ListTransactionsResponse> ListTransactionsAsync(AccessToken accessToken, string accountIdKey)
        {
            return await GetAsync<ListTransactionsResponse>(ListTransactionsResponse.RequestParameters(accountIdKey), accessToken);
        }

        public async Task<ViewPortfolio_PerformanceViewResponse> ViewPortfolioPerformanceAsync(AccessToken accessToken, string accountIdKey)
        {
            return await GetAsync<ViewPortfolio_PerformanceViewResponse>(ViewPortfolio_PerformanceViewResponse.RequestParameters(accountIdKey), accessToken);
        }

        public enum GetQuotesDetailFlag
        {
            ALL, FUNDAMENTAL, INTRADAY, OPTIONS, WEEK_52, MF_DETAIL
        }

        public async Task<GetQuotesResponse> GetQuotesAsync(AccessToken accessToken, List<string> symbols, GetQuotesDetailFlag detailFlag = GetQuotesDetailFlag.INTRADAY)
        {
            return await GetAsync<GetQuotesResponse>(GetQuotesResponse.RequestParameters(symbols, detailFlag.ToString()), accessToken);
        }

        /// <summary>
        /// for now all symbols are valid.
        /// need to hook this up to a call to validate
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> ValidateSymbolAsync(string symbol)
        {
            return true;
        }

        public async Task<OrdersListResponse> GetOrdersAsync(AccessToken accessToken, string accountIdKey, string symbol)
        {
            return await GetAsync<OrdersListResponse>(OrdersListResponse.RequestParameters(accountIdKey, symbol), accessToken);
        }

        public async Task<PreviewOrderResponse> PreviewOrder(AccessToken accessToken, string accountIdKey, PreviewOrderResponse.RequestBody requestBody)
        {
            PreviewOrderResponse requestData = new();
            requestData.RequestBodyData = requestBody;

            return await Post<PreviewOrderResponse, PreviewOrderResponse.RequestBody>(requestData, PreviewOrderResponse.RequestParameters(accountIdKey), accessToken);
        }

        public async Task<PlaceOrderResponse> PlaceOrder(AccessToken accessToken, string accountIdKey, PlaceOrderResponse.RequestBody requestBody)
        {
            PlaceOrderResponse requestData = new();
            requestData.RequestBodyData = requestBody;

            return await Post<PlaceOrderResponse, PlaceOrderResponse.RequestBody>(requestData, PlaceOrderResponse.RequestParameters(accountIdKey), accessToken);
        }


    }
}