using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.Authorization.interfaces;
using TradeLogic.Authorization;

namespace TradeLogic
{
    public partial class Trader
    {
        RequestToken? requestToken;

        internal async Task<RequestToken> GetRequestToken()
        {
            return await _authorizationApi.GetRequestTokenAsync(RequestTokenUrl);
        }

        public string GetAuthorizationUrl()
        {
            if (requestToken == null || requestToken.Expired)
            {
                requestToken = GetRequestToken().Result;
            }
            return _authorizationApi.GetAuthorizeApplicationURL(requestToken, AuthorizeUrl);
        }

        public async Task<AccessToken> GetAccessToken(string verificationCode)
        {
            if (requestToken == null || requestToken.Expired)
            {
                requestToken = GetRequestToken().Result;
            }
            return await _authorizationApi.GetAccessTokenAsync(requestToken, verificationCode, AccessTokenUrl);
        }

        public async Task<AccessToken> RenewAccessToken(AccessToken accessToken)
        {
            return await _authorizationApi.RenewAccessToken(accessToken, RenewAccessTokenUrl);
        }

        public async Task<bool> RevokeAccessToken(AccessToken accessToken)
        {
            return await _authorizationApi.RevokeAccessToken(accessToken, RevokeAccessTokenURL);
        }

    }
}
