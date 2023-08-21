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

    }


}