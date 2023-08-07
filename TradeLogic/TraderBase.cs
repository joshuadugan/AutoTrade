using OAuth;
using System.Data.SqlTypes;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Security.Authentication;
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
            _ordersTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 46)),
            _accountsTokenBucket = new TokenBucket(2, new TimeSpan(0, 0, 0, 1, 102)),
            _quotesTokenBucket = new TokenBucket(4, new TimeSpan(0, 0, 0, 1, 46));

        internal string _ConsumerKey = string.Empty;
        internal string _ConsumerSecret = string.Empty;
        internal AuthorizationApi _authorizationApi;
        internal string _BaseURL = string.Empty;

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
                Console.WriteLine(oAuthRequest);
                Console.WriteLine(response);
                throw;
            }

            try
            {
                using (StringReader reader = new StringReader(responseString))
                {
                    var newObj = new XmlSerializer(resourceType).Deserialize(reader);
                    return (T)newObj;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Could not deserialize following response to {resourceType.ToString()}");
                Console.WriteLine(responseString);
                throw;
            }

        }

        private string GetUrl<T>(Dictionary<string, string> queryData) where T : IResource, new()
        {
            string resourceName = new T().GetResourceName();
            return (_BaseURL) + (queryData != null ? resourceName.Inject(queryData) : resourceName);
        }

        private async Task<TResult> Post<TRequest, TResult>(TRequest request, Dictionary<string, string> queryData, AccessToken accessToken) where TRequest : IResource, IRequest, new()
        {
            var resourceType = typeof(TResult);

            string url = GetUrl<TRequest>(queryData);

            var serializer = new XmlSerializer(resourceType);

            ObeyRequestRateLimits(resourceType);

            try
            {
                using HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
                var content = JsonContent.Create(request);
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                using Stream responseStream = await response.Content.ReadAsStreamAsync();
                using MemoryStream responseMemoryStream = new MemoryStream();
                responseStream.CopyTo(responseMemoryStream);
                responseMemoryStream.Position = 0;

                try
                {
                    return (TResult)serializer.Deserialize(responseMemoryStream);
                }
                catch (InvalidOperationException ex)
                {
                    responseStream.Position = 0;

                    using (var streamReader = new StreamReader(responseMemoryStream))
                    {
                        throw new DeserializeException(
                            ex.Message,
                            streamReader.ReadToEnd(),
                            ex
                        );
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("401"))
                {
                    throw new AuthenticationException(ex.Message, ex);
                }

                throw;
            }
        }

        internal static HttpClient GetHttpClientWithOauthHeader(OAuthRequest oAuthRequest)
        {
            string auth = oAuthRequest.GetAuthorizationHeader();
            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Add("Authorization", auth);
            return httpClient;
        }
    }
}