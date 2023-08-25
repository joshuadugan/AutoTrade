using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic;
using TradeLogic.Authorization;

namespace AutoTradeMobile.ViewModels
{
    public partial class TradePageViewModel : ObservableObject
    {
        public TradeApp Trade { get; private set; }
        public void Load(TradeApp trade)
        {
            Trade = trade;
            Trade.ReplayLastSession = ReplayLastSession;
            Trade.SimulateOrders = SimulateOrders;
            Symbol = Trade.StoredData.LastSymbol;

        }

        [RelayCommand]
        public void LoadAccountsAsync()
        {
            if (Trade.AccessToken != null)
            {
                Trade.LoadAccountsAsync();
            }
            else
            {
                ErrorMessage = "No Access Token";
            }
        }


        [RelayCommand]
        public void MAImageClick(string args)
        {
            if (args != null)
            {
                var vals = args.Split(",");
                bool increase = bool.Parse(vals[0]);
                int index = int.Parse(vals[1]);
                var study = Trade.SymbolData.Studies[index];
                if (increase)
                {
                    study.Period++;
                }
                else
                {
                    study.Period--;
                }
            }
        }

        [ObservableProperty]
        string errorMessage;

        [ObservableProperty]
        bool hasError;

        [ObservableProperty]
        string symbol;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTraderStopped))]
        bool isTraderRunning;

        public bool IsTraderStopped
        {
            get => !IsTraderRunning;
        }

        [ObservableProperty]
        bool hasValidAccessToken;

        [ObservableProperty]
        SymbolData tradingData;

        public bool ReplayLastSession { get; set; }
        public bool SimulateOrders { get; set; }

        [ObservableProperty]
        bool requireAccountId;

        [RelayCommand]
        public void StartTrading()
        {
            StartTradingInternal();
        }

        public void AccountSelected(Account account)
        {
            Trade.StoredData.LastAccount = account;
            RequireAccountId = false;

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

                //validate the AccountId
                String AccountIdKey = Trade.StoredData.LastAccount?.AccountIdKey;
                if (string.IsNullOrEmpty(AccountIdKey))
                {
                    LoadAccountsAsync();
                    RequireAccountId = true;
                    return;
                }

                Trade.StoredData.LastSymbol = Symbol;
                Trade.StoredData.LastAccessToken = Trade.AccessToken;
                IsTraderRunning = true;
                //startup the trader with this symbol
                Trade.StartTradingSymbolAsync(Symbol);
                //get the trade data object reference for the page context
                TradingData = Trade.SymbolData;

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
