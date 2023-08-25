using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoTradeMobile.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {

        public TradeApp Trade { get; private set; }
        public void SetTradeReference(TradeApp trade)
        {
            Trade = trade;
        }

        [ObservableProperty]
        string errorMessage = string.Empty;

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        bool showVerifyPrompt;

        [ObservableProperty]
        bool showSetupControls;

        [ObservableProperty]
        string verificationCode;

        internal void HandleError(Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        internal void ClearError()
        {
            ErrorMessage = string.Empty;
        }

        public String AuthKey
        {
            get { return Trade.AuthData.AuthKey; }
            set { Trade.AuthData.AuthKey = value; }
        }
        public String AuthSecret
        {
            get { return Trade.AuthData.AuthSecret; }
            set { Trade.AuthData.AuthSecret = value; }
        }

        [RelayCommand]
        public async Task Login()
        {

            IsBusy = true;
            ShowVerifyPrompt = false;

            try
            {

                if (Trade.AuthData.isConfigured)
                {
                    ShowSetupControls = false;
                }
                else
                {
                    //show the textboxes for entering the app key and secret
                    IsBusy = false;
                    ShowSetupControls = true;
                    return;
                }

                bool BrowserOpened = await Trade.StartAuthProcessAsync();
                if (BrowserOpened)
                {
                    //show the auth code textbox
                    ShowVerifyPrompt = true;
                    IsBusy = false;

                }

            }
            catch (Exception ex)
            {
                IsBusy = false;
                HandleError(ex);
            }

        }

        [RelayCommand]
        public void Setup()
        {
            ShowSetupControls = true;
        }

        [RelayCommand]
        public async Task Verify()
        {
            try
            {
                IsBusy = true;

                bool isAuthorized = await Trade.VerifyCodeAsync(VerificationCode);
                if (isAuthorized)
                {
                    //navigate to the trading page
                    IsBusy = false;
                    await Shell.Current.GoToAsync("TradePage");
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                HandleError(ex);
            }
        }

        public bool ReplayLastSession { get; set; }
        public bool SimulateOrders { get; set; }

        [RelayCommand]
        public async Task SimulateLastSession()
        {
            await Shell.Current.GoToAsync("TradePage");
        }

    }
}
