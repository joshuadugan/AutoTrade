using Microsoft.VisualBasic;

namespace AutoTradeMobile
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        internal void HandleError(Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.IsVisible = true;
        }
        internal void ClearError()
        {
            lblError.Text = string.Empty;
            lblError.IsVisible = false;
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            ActivityControl.IsRunning = true;
            BtnVerification.IsVisible = false;
            txtVerificationCode.IsVisible = false;

            try
            {

                if (TradeApp.AuthData.isConfigured)
                {
                    txtKey.IsVisible = false;
                    txtSecret.IsVisible = false;
                    lblDevKey.IsVisible = false;
                }
                else
                {
                    //show the textboxes for entering the app key and secret
                    ActivityControl.IsRunning = false;
                    txtKey.IsVisible = true;
                    txtSecret.IsVisible = true;
                    lblDevKey.IsVisible = true;
                    return;
                }

                bool BrowserOpened = await TradeApp.StartAuthProcessAsync(useSandBox: true);
                if (BrowserOpened)
                {
                    //show the auth code textbox
                    txtVerificationCode.IsVisible = true;
                    BtnVerification.IsVisible = true;
                    ActivityControl.IsRunning = false;

                }

            }
            catch (Exception ex)
            {
                ActivityControl.IsRunning = false;
                HandleError(ex);
            }

        }

        private void btnEditKeys_Clicked(object sender, EventArgs e)
        {
            txtKey.IsVisible = true;
            txtSecret.IsVisible = true;
            lblDevKey.IsVisible = true;
        }

        private async void BtnVerification_Clicked(object sender, EventArgs e)
        {
            try
            {
                ActivityControl.IsRunning = true;

                bool isAuthorized = await TradeApp.VerifyCodeAsync(txtVerificationCode.Text);
                if (isAuthorized)
                {
                    //navigate to the trading page
                    ActivityControl.IsRunning = false;
                    await Shell.Current.GoToAsync("TradePage");
                }
            }
            catch (Exception ex)
            {
                ActivityControl.IsRunning = false;
                HandleError(ex);
            }
        }

        private async void btnTesting_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TradePage");
        }
    }
}