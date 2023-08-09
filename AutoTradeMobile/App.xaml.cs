namespace AutoTradeMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Routing.RegisterRoute("TradePage", typeof(TradePage));

        }
    }
}