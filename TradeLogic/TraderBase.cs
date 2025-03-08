using OAuth;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Security.Authentication;
using System.Text;
using System.Xml.Serialization;
using TradeLogic.Authorization;
using TradeLogic.Authorization.interfaces;
using TradeLogic.Exceptions;
using TradeLogic.extensions;

namespace TradeLogic
{
    public class TraderBase
    {

        // Rate Limits:
        // Orders          2 incoming requests per second per user, 7000 per hour
        // Accounts        2 incoming requests per second per user, 7000 per hour
        // Quotes          4 incoming requests per second per user, 14000 per hour
        // Notifications   2 incoming requests per second (per user?)
        private readonly static TokenBucket
            _ordersTokenBucket = new TokenBucket(2, TimeSpan.FromSeconds(1)),   // 2 orders per second
            _accountsTokenBucket = new TokenBucket(2, TimeSpan.FromSeconds(1)), // 2 account requests per second
            _quotesTokenBucket = new TokenBucket(4, TimeSpan.FromSeconds(1));   // 4 quotes per second

        internal string _ConsumerKey = string.Empty;
        internal string _ConsumerSecret = string.Empty;
        internal AuthorizationApi _authorizationApi;
        internal string _BaseURL = string.Empty;
        private static readonly HttpClient _httpClient = new HttpClient();

        public TraderBase(string ConsumerKey, string ConsumerSecret)
        {
            _ConsumerKey = ConsumerKey;
            _ConsumerSecret = ConsumerSecret;
            _authorizationApi = new AuthorizationApi(_ConsumerKey, _ConsumerSecret);
            _authorizationApi.Realm = string.Empty;
        }

        internal bool ProductionMode { get; set; } = true;

        private static void ObeyRequestRateLimits(Type t)
        {
            if (t is IBelongToAccountService)
            {
                // get from accounts bucket
                _accountsTokenBucket.Consume();
            }
            else if (t is IBelongToMarketService)
            {
                // get from market bucket
                _quotesTokenBucket.Consume();
            }
            else if (t is IBelongToOrderService)
            {
                // get from order bucket
                _ordersTokenBucket.Consume();
            }
        }

        internal async Task<T> GetAsync<T>(Dictionary<string, string> queryData, AccessToken accessToken) where T : IResource, new()
        {
            var resourceType = typeof(T);

            string url = GetUrl<T>(queryData);

            ObeyRequestRateLimits(resourceType);

            OAuthRequest oAuthRequest = new()
            {
                Method = "GET",
                Type = OAuthRequestType.ProtectedResource,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _ConsumerKey,
                ConsumerSecret = _ConsumerSecret,
                RequestUrl = url,
                Token = accessToken.Token,
                TokenSecret = accessToken.TokenSecret,
            };

            using HttpClient httpClient = TraderBase.GetHttpClientWithOauthHeader(oAuthRequest);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            string responseString = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                Trace.WriteLine(oAuthRequest);
                Trace.WriteLine(response);
                throw;
            }

            try
            {
                if (string.IsNullOrEmpty(responseString))
                {
                    return default(T);
                }
                using (StringReader reader = new StringReader(responseString))
                {
                    var newObj = new XmlSerializer(resourceType).Deserialize(reader);
                    return (T)newObj;
                }
            }
            catch (Exception)
            {
                Trace.WriteLine($"Could not deserialize following response to {resourceType.ToString()}");
                Trace.WriteLine(responseString);
                throw;
            }

        }

        private string GetUrl<T>(Dictionary<string, string> queryData) where T : IResource, new()
        {
            string resourceName = new T().GetResourceName();
            return (_BaseURL) + (queryData != null ? resourceName.Inject(queryData) : resourceName);
        }

        internal async Task<TRequestAndResult> Post<TRequestAndResult, TRequestBody>(
                                                    TRequestAndResult request,
                                                    Dictionary<string, string> queryData,
                                                    AccessToken accessToken)
                                                    where TRequestAndResult : IResource, IRequest<TRequestBody>, new()
        {
            string url = GetUrl<TRequestAndResult>(queryData);
            ObeyRequestRateLimits(typeof(TRequestAndResult));

            // Serialize request body to XML
            var requestBodySerializer = new XmlSerializer(typeof(TRequestBody));
            string requestBodyXml;
            using (var stringWriter = new StringWriter())
            {
                requestBodySerializer.Serialize(stringWriter, request.ToRequestBodyObject());
                requestBodyXml = stringWriter.ToString();
            }

            // Create OAuth request
            OAuthRequest oAuthRequest = new()
            {
                Method = "POST",
                Type = OAuthRequestType.ProtectedResource,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _ConsumerKey,
                ConsumerSecret = _ConsumerSecret,
                RequestUrl = url,
                Token = accessToken.Token,
                TokenSecret = accessToken.TokenSecret,
            };

            using HttpClient httpClient = TraderBase.GetHttpClientWithOauthHeader(oAuthRequest);

            var content = new StringContent(requestBodyXml, Encoding.UTF8, "application/xml");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            var responseSerializer = new XmlSerializer(typeof(TRequestAndResult));
            return (TRequestAndResult)responseSerializer.Deserialize(responseStream);
        }

        internal static HttpClient GetHttpClientWithOauthHeader(OAuthRequest oAuthRequest)
        {
            string auth = oAuthRequest.GetAuthorizationHeader();
            _httpClient.DefaultRequestHeaders.Add("Authorization", auth);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            return _httpClient;
        }
    }
}