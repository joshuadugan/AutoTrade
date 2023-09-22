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

        public TradeApp Trade
        {
            get
            {
                return App.Trade;
            }
        }

        public SymbolData Symbol
        {
            get
            {
                return TradeApp.Symbol;
            }
        }

        public OrderData OrderState
        {
            get
            {
                return TradeApp.OrderState;
            }
        }

        public PersistedData Settings
        {
            get
            {
                return TradeApp.Settings;
            }
        }

        public AuthDataContainer AuthData
        {
            get
            {
                return TradeApp.AuthData;
            }
        }

        public TradePageViewModel()
        {
            SymbolName = TradeApp.Settings.LastSymbol;
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
                var study = TradeApp.Symbol.Studies[index];
                if (increase)
                {
                    study.Period++;
                }
                else
                {
                    study.Period--;
                }
                TradeApp.Symbol.SaveStudies();
            }
        }

        [RelayCommand]
        public void PriceImageClick(string args)
        {
            if (args != null)
            {
                var vals = args.Split(",");
                bool increase = bool.Parse(vals[0]);
                int index = int.Parse(vals[1]);
                var study = TradeApp.Symbol.Studies[index];
                if (increase)
                {
                    study.UptrendAmountRequired += .01m;
                }
                else
                {
                    study.UptrendAmountRequired -= .01m;
                }
            }
        }

        [RelayCommand]
        public void VelocityOrderImageClick(string args)
        {
            if (args != null)
            {
                bool increase = bool.Parse(args);
                if (increase)
                {
                    TradeApp.Symbol.VelocityTradeOrderValue += .01m;
                }
                else
                {
                    TradeApp.Symbol.VelocityTradeOrderValue -= .01m;
                }
                TradeApp.Symbol.SaveVelocitySettings();
            }
        }

        [RelayCommand]
        public void VelocityStopImageClick(string args)
        {
            if (args != null)
            {
                bool increase = bool.Parse(args);
                if (increase)
                {
                    TradeApp.Symbol.VelocityTradeTrailingStopValue += .01m;
                }
                else
                {
                    TradeApp.Symbol.VelocityTradeTrailingStopValue -= .01m;
                }
                TradeApp.Symbol.SaveVelocitySettings();
            }
        }

        public bool ShowRestartSimulationButton
        {
            get
            {
                return TradeApp.ReplayLastSession;
            }
        }
        [RelayCommand]
        public void RestartSimulation()
        {
            Trade.StopTrading();
            Trade.ResetState();
            Trade.StartTrading();
        }

        [ObservableProperty]
        string pageTitle = "Trade";

        [ObservableProperty]
        string errorMessage;

        [ObservableProperty]
        bool hasError;

        [ObservableProperty]
        string symbolName;

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
        bool requireAccountId;

        [RelayCommand]
        public void StartTrading()
        {
            StartTradingInternal();
        }

        public void AccountSelected(Account account)
        {
            TradeApp.Settings.LastAccount = account;
            RequireAccountId = false;

        }

        private void StartTradingInternal()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SymbolName)) { throw new Exception("No Symbol"); }

                //validate the AccountId
                String AccountIdKey = TradeApp.Settings.LastAccount?.AccountIdKey;
                if (string.IsNullOrEmpty(AccountIdKey))
                {
                    LoadAccountsAsync();
                    RequireAccountId = true;
                    return;
                }

                TradeApp.Settings.LastSymbol = SymbolName;
                TradeApp.Settings.LastAccessToken = Trade.AccessToken;
                IsTraderRunning = true;
                //startup the trader with this symbol
                Trade.StartTradingSymbolAsync(SymbolName);
                PageTitle = SymbolName;
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
