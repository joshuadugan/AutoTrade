using AutoTradeMobile.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;

namespace AutoTradeMobile
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            GoAsync();
        }


        public async Task GoAsync()
        {
            await Task.Delay(500);
            await Shell.Current.GoToAsync("TradeTabPage");
        }

        [RelayCommand]
        public void Go()
        {
            GoAsync();
        }


    }
}