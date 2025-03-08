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

        internal async Task<RequestToken> GetRequestTokenAsync()
        {
            return await _authorizationApi.GetRequestTokenAsync(RequestTokenUrl);
        }

        public async Task<string> GetAuthorizationUrlAsync()
        {
            if (requestToken == null || requestToken.Expired)
            {
                requestToken = await GetRequestTokenAsync();
            }
            return _authorizationApi.GetAuthorizeApplicationURL(requestToken, AuthorizeUrl);
        }

        public async Task<AccessToken> GetAccessToken(string verificationCode)
        {
            if (requestToken == null || requestToken.Expired)
            {
                requestToken = await GetRequestTokenAsync();
            }
            return await _authorizationApi.GetAccessTokenAsync(requestToken, verificationCode, AccessTokenUrl);
        }

        public async Task<AccessToken> RenewAccessTokenAsync(AccessToken accessToken)
        {
            return await _authorizationApi.RenewAccessTokenAsync(accessToken, RenewAccessTokenUrl);
        }

        public async Task<bool> RevokeAccessToken(AccessToken accessToken)
        {
            return await _authorizationApi.RevokeAccessTokenAsync(accessToken, RevokeAccessTokenURL);
        }

        public async Task<AccessToken> EnsureTokenIsValidAsync(AccessToken accessToken)
        {
            if (accessToken.IsExpired) // Assumes IsExpired checks the token's expiration time
            {
                accessToken = await RenewAccessTokenAsync(accessToken);
            }
            return accessToken;
        }
    }
}
