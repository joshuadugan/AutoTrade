using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.Authorization.interfaces;

namespace TradeLogic.Authorization
{
    internal static class OAuthTokenExtensions
    {

        public static bool IsSet(this OAuthToken token)
        {
            return (token != null && !string.IsNullOrWhiteSpace(token.Token) && !string.IsNullOrWhiteSpace(token.TokenSecret));
        }

        public static RequestToken ToRequestToken(this OAuthToken requestToken)
        {
            return
                requestToken == null ? new RequestToken() :
                new RequestToken
                {
                    ConsumerKey = requestToken.ConsumerKey,
                    Expires = DateTime.Now.AddMinutes(4).AddSeconds(45),
                    Realm = requestToken.Realm,
                    SessionHandle = requestToken.SessionHandle,
                    Token = requestToken.Token,
                    TokenSecret = requestToken.TokenSecret
                };
        }

        public static AccessToken ToAccessToken(this OAuthToken token)
        {
            return
                token == null ? new AccessToken() :
                new AccessToken
                {
                    ConsumerKey = token.ConsumerKey,
                    Realm = token.Realm,
                    SessionHandle = token.SessionHandle,
                    Token = token.Token,
                    TokenSecret = token.TokenSecret
                };
        }

        /// <summary>
        /// Outputs the token value
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string TokenToString(this OAuthToken token)
        {
            return token == null ? "Token is null" : $"Token:{token.Token}";
        }

    }
}
