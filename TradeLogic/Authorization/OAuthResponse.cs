using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.Authorization
{
    internal class OAuthResponse
    {
        public string? oauth_token { get; set; } = string.Empty;

        public string? oauth_token_secret { get; set; } = string.Empty;

        public bool oauth_callback_confirmed { get; set; } 

        public OAuthToken ToOAuthToken()
        {
            return new OAuthToken()
            {
                Token = oauth_token,
                TokenSecret = oauth_token_secret,
            };
        }
    }
}
