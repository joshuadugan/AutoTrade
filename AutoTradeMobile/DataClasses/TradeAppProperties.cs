using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    public partial class TradeApp : ObservableObject
    {
        public bool useSandBox { get; set; }
        public AuthDataContainer AuthData { get; } = new();
        public PersistedData StoredData { get; } = new();
        public TradingSessionData SessionData { get; } = new();
        public ConcurrentDictionary<string, SymbolData> Symbols { get; } = new();

        TradeLogic.Trader _trader;
        public TradeLogic.Trader trader
        {
            get
            {
                if (_trader == null)
                {
                    _trader = new TradeLogic.Trader(AuthData.AuthKey, AuthData.AuthSecret, useSandBox);
                }
                return _trader;
            }
            set
            {
                _trader = value;
            }
        }
        public AccessToken accessToken { get; set; }
        public List<string> currentSymbolList { get; } = new();
        public Timer TickerTimer { get; set; }

    }

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
    public class PersistedData
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

        public AccessToken LastAccessToken
        {
            get
            {
                var AccessTokenJson = Preferences.Get(nameof(LastAccessToken), null);
                if (AccessTokenJson != null)
                {
                    try
                    {
                        AccessToken at = JsonSerializer.Deserialize<AccessToken>(AccessTokenJson);
                        return at;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Unable to deserialize access token {ex.Message}");
                    }
                }
                return null;
            }
            set
            {
                string jsonString = JsonSerializer.Serialize(value);
                Preferences.Set(nameof(LastAccessToken), jsonString);
            }
        }

    }
    public partial class TradingSessionData : ObservableObject
    {

        [ObservableProperty]
        int totalRequests;

        [ObservableProperty]
        TimeSpan lastQuoteResponseTime;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        string tradingError;

        public bool HasError
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TradingError);
            }
        }
    }

}