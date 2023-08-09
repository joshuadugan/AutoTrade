using System.Collections.Concurrent;
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    internal partial class TradeApp
    {
        internal static AuthDataContainer AuthData { get; } = new();
        internal class AuthDataContainer
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
        private static ConcurrentDictionary<string, SymbolData> Symbols { get; set; } = new();
        private static TradeLogic.Trader trader { get; set; }
        private static AccessToken accessToken { get; set; }
        private static List<string> currentSymbolList { get; set; } = new();
        private static Timer TickerTimer { get; set; }

    }
}