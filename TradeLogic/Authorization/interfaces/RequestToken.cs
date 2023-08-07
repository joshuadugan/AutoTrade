using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLogic.Authorization.interfaces
{
    public class RequestToken : OAuthToken
    {
        public DateTime Expires { get; set; }

        public bool Expired
        {
            get { return (Expires <= DateTime.Now); }
        }
    }
}
