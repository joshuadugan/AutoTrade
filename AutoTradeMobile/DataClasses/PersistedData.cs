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
    public class PersistedData
    {
        public Account LastAccount
        {
            get
            {
                var JsonString = Preferences.Get(nameof(LastAccount), null);
                if (JsonString != null)
                {
                    try
                    {
                        Account a = JsonSerializer.Deserialize<Account>(JsonString);
                        return a;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Unable to deserialize account {ex.Message}");
                    }
                }
                return null;
            }
            set
            {
                string jsonString = JsonSerializer.Serialize(value);
                Preferences.Set(nameof(LastAccount), jsonString);
            }
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

        public decimal LastVelocityTradeOrderValue
        {
            get => Decimal.Parse(Preferences.Get(nameof(LastVelocityTradeOrderValue), "0"));
            set => Preferences.Set(nameof(LastVelocityTradeOrderValue), value.ToString());
        }

        public decimal LastVelocityTradeTrailingStopValue
        {
            get => Decimal.Parse(Preferences.Get(nameof(LastVelocityTradeTrailingStopValue), "0"));
            set => Preferences.Set(nameof(LastVelocityTradeTrailingStopValue), value.ToString());
        }

        public bool PlaceTrailingStopWhenTradeIsProfitable
        {
            get => bool.Parse(Preferences.Get(nameof(PlaceTrailingStopWhenTradeIsProfitable), "false"));
            set => Preferences.Set(nameof(PlaceTrailingStopWhenTradeIsProfitable), value.ToString());
        }

        public bool SimulateMarketDataFromFile {
            get => bool.Parse(Preferences.Get(nameof(SimulateMarketDataFromFile), "false"));
            set => Preferences.Set(nameof(SimulateMarketDataFromFile), value.ToString());
        }

        public bool SimulateOrders {
            get => bool.Parse(Preferences.Get(nameof(SimulateOrders), "false"));
            set => Preferences.Set(nameof(SimulateOrders), value.ToString());
        }

    }


}