using AutoTradeMobile.ViewModels;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;

namespace AutoTradeMobile
{
    public partial class MainPage : ContentPage
    {

        public MainPageViewModel ViewModel { get; }

        public TradeApp Trade
        {
            get
            {
                return App.Trade;
            }
        }

        public MainPage(MainPageViewModel mainPageViewModel)
        {
            ViewModel = mainPageViewModel;

            InitializeComponent();

            BindingContext = ViewModel;
            
        }

        
        
       
    }
}