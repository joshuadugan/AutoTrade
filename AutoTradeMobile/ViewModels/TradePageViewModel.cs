using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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
        public void SetTradeReference(TradeApp trade)
        {
            Trade = trade;
            Symbol = Trade.StoredData.LastSymbol;
            if (Trade.accessToken == null)
            {
                AccessToken at = Trade.StoredData.LastAccessToken;
                if (at != null && at.Expired == false)
                {
                    Trade.accessToken = at;
                    HasValidAccessToken = true;
                    StartTrading();
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

        [RelayCommand]
        public void StartTrading()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Symbol)) { throw new Exception("No Symbol"); }

                Trade.StoredData.LastSymbol = Symbol;
                Trade.StoredData.LastAccessToken = Trade.accessToken;
                IsTraderRunning = true;
                //startup the trader with this symbol
                Trade.StartTradingSymbolAsync(Symbol);
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

        [RelayCommand]
        public void Debug()
        {
            Trace.WriteLine(TradingData);
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
