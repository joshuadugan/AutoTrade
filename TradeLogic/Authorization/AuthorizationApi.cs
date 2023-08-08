using OAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TradeLogic.Authorization.interfaces;
using TradeLogic.extensions;

namespace TradeLogic.Authorization
{
    internal class AuthorizationApi : iAuthorizationApi
    {
        private readonly string _ConsumerKey;
        private readonly string _ConsumerSecret;

        public string? Realm { get; internal set; }

        public AuthorizationApi(string ConsumerKey, string ConsumerSecret)
        {
            _ConsumerKey = ConsumerKey;
            _ConsumerSecret = ConsumerSecret;
            Realm = string.Empty;
        }

        public async Task<RequestToken> GetRequestTokenAsync(string requestTokenUrl)
        {

            OAuthRequest oAuthRequest = new()
            {
                Method = "GET",
                Type = OAuthRequestType.RequestToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _ConsumerKey,
                ConsumerSecret = _ConsumerSecret,
                RequestUrl = requestTokenUrl,
                Realm = string.Empty,
                //Version = "1.0",
                CallbackUrl = "oob"
            };

            using HttpClient httpClient = TraderBase.GetHttpClientWithOauthHeader(oAuthRequest);

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(oAuthRequest.RequestUrl);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                NameValueCollection parsedValues = HttpUtility.ParseQueryString(responseString);

                OAuthResponse requestTokenResponse = new()
                {
                    oauth_token = parsedValues.Get("oauth_token"),
                    oauth_token_secret = parsedValues.Get("oauth_token_secret"),
                    oauth_callback_confirmed = false
                };

                if (bool.TryParse(parsedValues["oauth_callback_confirmed"], out bool oauth_callback_confirmed))
                {
                    requestTokenResponse.oauth_callback_confirmed = oauth_callback_confirmed;
                }

                return requestTokenResponse.ToOAuthToken().ToRequestToken();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP Request Exception:");
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public string GetAuthorizeApplicationURL(RequestToken requestToken, string AuthorizeUrl)
        {
            if (string.IsNullOrWhiteSpace(requestToken.Token) || string.IsNullOrWhiteSpace(AuthorizeUrl))
            {
                throw new Exception("No Request Token");
            }

            var parameters = new Dictionary<string, string>
            {
                { "key", _ConsumerKey },
                { "token", requestToken.Token }
            };

            string queryString = string.Join("&", parameters.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));

            string urlWithQueryString = $"{AuthorizeUrl}?{queryString}";

            return urlWithQueryString;
        }

        public async Task<AccessToken> GetAccessTokenAsync(RequestToken requestToken, string oauth_verifier, string AccessTokenUrl)
        {
            OAuthRequest oAuthRequest = new()
            {
                Method = "GET",
                Type = OAuthRequestType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _ConsumerKey,
                ConsumerSecret = _ConsumerSecret,
                RequestUrl = AccessTokenUrl,
                Token = requestToken.Token,
                TokenSecret = requestToken.TokenSecret,
                Verifier = oauth_verifier,
            };

            using HttpClient httpClient = TraderBase.GetHttpClientWithOauthHeader(oAuthRequest);

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(oAuthRequest.RequestUrl);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                NameValueCollection parsedValues = HttpUtility.ParseQueryString(responseString);

                OAuthResponse accessTokenResponse = new()
                {
                    oauth_token = parsedValues.Get("oauth_token"),
                    oauth_token_secret = parsedValues.Get("oauth_token_secret"),
                    oauth_callback_confirmed = false
                };

                return accessTokenResponse.ToOAuthToken().ToAccessToken();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP Request Exception:");
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<AccessToken> RenewAccessTokenAsync(AccessToken accessToken, string RenewAccessTokenUrl)
        {
            OAuthRequest oAuthRequest = new()
            {
                Method = "GET",
                Type = OAuthRequestType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _ConsumerKey,
                ConsumerSecret = _ConsumerSecret,
                RequestUrl = RenewAccessTokenUrl,
                Token = accessToken.Token,
                TokenSecret = accessToken.TokenSecret,
                Realm = string.Empty,
            };

            using HttpClient httpClient = TraderBase.GetHttpClientWithOauthHeader(oAuthRequest);

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(oAuthRequest.RequestUrl);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                if (responseString == "Access Token has been renewed")
                {
                    return accessToken;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP Request Exception:");
                Console.WriteLine(ex.Message);
                throw;
            }

            return new AccessToken();
        }

        public async Task<bool> RevokeAccessTokenAsync(AccessToken accessToken, string RevokeAccessTokenUrl)
        {
            OAuthRequest oAuthRequest = new()
            {
                Method = "GET",
                Type = OAuthRequestType.AccessToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _ConsumerKey,
                ConsumerSecret = _ConsumerSecret,
                RequestUrl = RevokeAccessTokenUrl,
                Token = accessToken.Token,
                TokenSecret = accessToken.TokenSecret,
                Realm = string.Empty,
            };

            using HttpClient httpClient = TraderBase.GetHttpClientWithOauthHeader(oAuthRequest);

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(oAuthRequest.RequestUrl);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                if (responseString == "Revoked Access Token")
                {
                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP Request Exception:");
                Console.WriteLine(ex.Message);
                return false;
            }

            return false;
        }


    }
}
