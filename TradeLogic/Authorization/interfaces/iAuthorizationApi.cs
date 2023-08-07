using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.Authorization.interfaces
{
    internal interface iAuthorizationApi
    {
        Task<RequestToken> GetRequestTokenAsync(string requestTokenUrl);

        string GetAuthorizeApplicationURL(RequestToken requestToken, string AuthorizeUrl);

        Task<AccessToken> GetAccessTokenAsync(RequestToken requestToken, string verificationCode, string AccessTokenUrl);

        Task<AccessToken> RenewAccessToken(AccessToken accessToken, string RenewAccessTokenUrl);

        Task<bool> RevokeAccessToken(AccessToken accessToken, string RevokeAccessTokenUrl);

    }

    public interface IResource
    {
        string GetResourceName();
    }

    public interface IBelongToAuthorizationService
    {
    }

    public interface IBelongToAccountService
    {
    }

    public interface IBelongToMarketService
    {
    }

    public interface IBelongToOrderService
    {
    }

    public interface IBelongToNotificationService
    {
    }

    public interface IBelongToLimitService
    {
    }

    public interface IRequest
    {
        string ToXml();
    }

}
