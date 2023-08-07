using OAuth;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Principal;
using System.Xml.Serialization;
using TradeLogic.APIModels;
using TradeLogic.APIModels.Accounts;
using TradeLogic.APIModels.Accounts.portfolio;
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

        public async Task<AccountListResponse> ListAccounts(AccessToken accessToken)
        {
            return await GetAsync<AccountListResponse>(null, accessToken);
        }

        public async Task<AccountBalanceResponse> GetAccountBalances(AccessToken accessToken, string accountIdKey)
        {
            return await GetAsync<AccountBalanceResponse>(AccountBalanceResponse.RequestParameters(accountIdKey), accessToken);
        }

        public async Task<ListTransactionsResponse> ListTransactions(AccessToken accessToken, string accountIdKey)
        {
            return await GetAsync<ListTransactionsResponse>(ListTransactionsResponse.RequestParameters(accountIdKey), accessToken);
        }

        public async Task<ViewPortfolio_PerformanceViewResponse> ViewPortfolioPerformance(AccessToken accessToken, string accountIdKey)
        {
            return await GetAsync<ViewPortfolio_PerformanceViewResponse>(ViewPortfolio_PerformanceViewResponse.RequestParameters(accountIdKey), accessToken);
        }

        public enum GetQuotesDetailFlag
        {
            ALL, FUNDAMENTAL, INTRADAY, OPTIONS, WEEK_52, MF_DETAIL
        }

        public async Task<GetQuotesResponse> GetQuotes(AccessToken accessToken, List<string> symbols, GetQuotesDetailFlag detailFlag = GetQuotesDetailFlag.INTRADAY)
        {
            return await GetAsync<GetQuotesResponse>(GetQuotesResponse.RequestParameters(symbols, detailFlag.ToString()), accessToken);
        }

    }
}