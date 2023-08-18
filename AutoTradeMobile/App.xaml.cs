namespace AutoTradeMobile
{
    public partial class App : Application
    {
        public App(TradeApp _TradeApp)
        {

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NGaF1cVWhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEZjUX5ZcndVTmJZWUZ3Vg==");

            InitializeComponent();

            MainPage = new AppShell();

            Routing.RegisterRoute("TradePage", typeof(TradePage));

            Current.UserAppTheme = AppTheme.Light;

        }
    }
}