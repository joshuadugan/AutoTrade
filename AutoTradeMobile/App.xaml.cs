
using AutoTradeMobile.ViewModels;

namespace AutoTradeMobile
{
    public partial class App : Application
    {

        public static TradeApp Trade { get; } = new();

        public static TradePageViewModel TradePageVM { get; } = new();

        public App()
        {

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWXZccXVcR2ddU0Z+WEs=");

            InitializeComponent();

            MainPage = new AppShell();

            Routing.RegisterRoute("TradePage", typeof(TradePage));
            Routing.RegisterRoute("TradeTabPage", typeof(TradeTabPage));

            Current.UserAppTheme = AppTheme.Light;

        }
    }
}