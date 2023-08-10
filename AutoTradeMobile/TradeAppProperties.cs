using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
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

        internal static PersistedData AppData { get; } = new();
        internal class PersistedData
        {
            public string LastSymbol
            {
                get
                {
                    return Preferences.Get(nameof(LastSymbol), string.Empty);
                }
                set
                {
                    Preferences.Set(nameof(LastSymbol), value);
                }
            }

        }

        internal static TradingSessionData SessionData { get; } = new();
        internal class TradingSessionData : ObservableObject
        {
            int _TotalRequests;
            public int TotalRequests
            {
                get => _TotalRequests;
                set => SetProperty(ref _TotalRequests, value);
            }

            TimeSpan _LastQuoteResponseTime;
            public TimeSpan LastQuoteResponseTime
            {
                get => _LastQuoteResponseTime;
                set => SetProperty(ref _LastQuoteResponseTime, value);
            }

            DateTime _LastQuoteDate;
            public DateTime LastQuoteDate
            {
                get => _LastQuoteDate;
                set => SetProperty(ref _LastQuoteDate, value);
            }

            string _TradingError;
            public string TradingError
            {
                get => _TradingError;
                set
                {
                    SetProperty(ref _TradingError, value);
                    OnPropertyChanged(nameof(hasError));
                }
            }
            public bool hasError
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(TradingError);
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