using AutoTradeMobile.DataClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    public partial class TradeApp : ObservableObject
    {
        public bool UseSandBox { get; }
        public AuthDataContainer AuthData { get; } = new();
        public PersistedData StoredData { get; } = new();
        public ConcurrentDictionary<string, SymbolData> Symbols { get; } = new();
        public ObservableCollection<MarketOrder> Orders { get; private set; } = new();

        TradeLogic.Trader _trader;
        public TradeLogic.Trader Trader
        {
            get
            {
                if (_trader == null)
                {
                    _trader = new TradeLogic.Trader(AuthData.AuthKey, AuthData.AuthSecret, UseSandBox);
                }
                return _trader;
            }
            set
            {
                _trader = value;
            }
        }
        public AccessToken AccessToken { get; set; }
        public List<string> CurrentSymbolList { get; } = new();
        public string AccountId { get; private set; }
        private Timer TickerTimer { get; set; }
        private Timer OrdersTimer { get; set; }

        [ObservableProperty]
        int totalRequests;

        [ObservableProperty]
        TimeSpan lastQuoteResponseTime;

        [ObservableProperty]
        TimeSpan lastOrderResponseTime;

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
        public string LastAccountId
        {
            get => Preferences.Get(nameof(LastAccountId), string.Empty);
            set => Preferences.Set(nameof(LastAccountId), value);
        }
        public string LastSymbol
        {
            get => Preferences.Get(nameof(LastSymbol), string.Empty);
            set => Preferences.Set(nameof(LastSymbol), value);
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


}