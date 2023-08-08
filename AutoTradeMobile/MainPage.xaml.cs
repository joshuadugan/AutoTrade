using Microsoft.VisualBasic;

namespace AutoTradeMobile
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            ActivityControl.IsRunning = true;

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

                bool BrowserOpened = await TradeApp.StartAuthProcess(useSandBox: true);
                if (BrowserOpened)
                {
                    //show the auth code textbox
                    txtVerificationCode.IsVisible = true;
                    ActivityControl.IsRunning = false;

                }

            }
            catch (Exception ex)
            {
                ActivityControl.IsRunning = false;
                HandleError(ex);
            }

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

    }
}