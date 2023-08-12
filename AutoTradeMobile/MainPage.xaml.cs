using AutoTradeMobile.ViewModels;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;

namespace AutoTradeMobile
{
    public partial class MainPage : ContentPage
    {

        public MainPageViewModel ViewModel { get; }

        public TradeApp Trade { get; }

        public MainPage(MainPageViewModel mainPageViewModel, TradeApp tradeApp)
        {
            mainPageViewModel.SetTradeReference(tradeApp);
            ViewModel = mainPageViewModel;
            Trade = tradeApp;

            InitializeComponent();

            BindingContext = ViewModel;
            
        }

        
        
       
    }
}