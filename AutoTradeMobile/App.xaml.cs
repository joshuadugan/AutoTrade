namespace AutoTradeMobile
{
    public partial class App : Application
    {
        public App(TradeApp _TradeApp)
        {
            InitializeComponent();

            MainPage = new AppShell();

            Routing.RegisterRoute("TradePage", typeof(TradePage));

            Current.UserAppTheme = AppTheme.Light;

        }
    }
}