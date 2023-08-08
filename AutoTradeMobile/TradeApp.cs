using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile
{
    internal class TradeApp
    {
        internal static Dictionary<string, SymbolData> Symbols = new();

        internal static TradeLogic.Trader trader;

        internal static async Task<bool> StartAuthProcess(bool useSandBox)
        {

            bool OpenWindow = false;

            trader = new TradeLogic.Trader(AuthData.AuthKey, AuthData.AuthSecret, useSandBox);

            string AuthorizationUrl = await trader.GetAuthorizationUrlAsync();

            OpenWindow = await Browser.Default.OpenAsync(new Uri(AuthorizationUrl), BrowserLaunchMode.SystemPreferred);

            return OpenWindow;
        }


        public static AuthDataContainer AuthData { get; } = new();
        public class AuthDataContainer
        {
            public bool isConfigured
            {
                get
                {
                    return string.IsNullOrEmpty(AuthKey) == false & string.IsNullOrEmpty(AuthSecret) == false;
                }
            }
            public string AuthKey
            {
                get
                {
                    return Preferences.Get(nameof(AuthKey), string.Empty);
                }
                set
                {
                    Preferences.Set(nameof(AuthKey), value);
                }
            }
            public string AuthSecret
            {
                get
                {
                    return Preferences.Get(nameof(AuthSecret), string.Empty);
                }
                set
                {
                    Preferences.Set(nameof(AuthSecret), value);
                }
            }
        }

    }
}
