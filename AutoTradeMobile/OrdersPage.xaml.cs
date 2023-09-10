using AutoTradeMobile.ViewModels;
using System.Diagnostics;

namespace AutoTradeMobile;

public partial class OrdersPage : ContentPage
{

    public TradePageViewModel ViewModel
    {
        get
        {
            return App.TradePageVM;
        }
    }


    public OrdersPage()
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

}