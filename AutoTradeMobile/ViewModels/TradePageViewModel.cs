using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.Authorization;

namespace AutoTradeMobile.ViewModels
{
    public partial class TradePageViewModel : ObservableObject
    {
        public TradeApp Trade { get; private set; }
        public void Load(TradeApp trade)
        {
            Trade = trade;
            Symbol = Trade.StoredData.LastSymbol;
            AccountId = Trade.StoredData.LastAccountId;
            if (Trade.AccessToken == null)
            {
                AccessToken at = Trade.StoredData.LastAccessToken;
                if (at != null && at.Expired == false)
                {
                    Trade.AccessToken = at;
                    HasValidAccessToken = true;
                    //StartTradingInternal();
                }
            }

            if (Trade.AccessToken != null)
            {
                //load the accounts
                Task.Run(() =>
                {
                    Accounts = Trade.GetAccountsAsync().Result.Accounts.Account.Select(a => new DataClasses.Account(a)).ToObservableCollection();
                });
            }

        }

        ObservableCollection<DataClasses.Account> Accounts;

        [ObservableProperty]
        string errorMessage;

        [ObservableProperty]
        bool hasError;

        [ObservableProperty]
        string symbol;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTraderStopped))]
        bool isTraderRunning;

        [ObservableProperty]
        string accountId;

        public bool IsTraderStopped
        {
            get => !IsTraderRunning;
        }

        [ObservableProperty]
        bool hasValidAccessToken;

        [ObservableProperty]
        SymbolData tradingData;

        [ObservableProperty]
        bool replayLastSession;

        [RelayCommand]
        public void StartTrading()
        {
            StartTradingInternal();
        }

        private void StartTradingInternal()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Symbol)) { throw new Exception("No Symbol"); }

                if (!ReplayLastSession)
                {
                    Helpers.WriteTextToFileAsync("", $"{Symbol}.txt");
                }

                Trade.StoredData.LastSymbol = Symbol;
                Trade.StoredData.LastAccessToken = Trade.AccessToken;
                IsTraderRunning = true;
                //startup the trader with this symbol
                Trade.StartTradingSymbolAsync(Symbol, AccountId, ReplayLastSession);
                //get the trade data object reference for the page context
                TradingData = Trade.GetSymbolData(Symbol);

            }
            catch (Exception ex)
            {
                IsTraderRunning = false;
                Trade.StopTrading();
                HandleError(ex);
            }
        }


        internal void HandleError(Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        internal void ClearError()
        {
            HasError = false;
            ErrorMessage = string.Empty;
        }

    }
}
