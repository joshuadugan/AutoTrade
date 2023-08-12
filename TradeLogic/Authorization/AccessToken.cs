using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.Authorization
{
    public class AccessToken : OAuthToken
    {
        public AccessToken()
        {
            Expires = DateTime.Now.AddHours(1).AddMinutes(45);
        }
        public DateTime Expires { get; set; }

        public bool Expired
        {
            get { return (Expires <= DateTime.Now); }
        }

    }
}
