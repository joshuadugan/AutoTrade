using AutoTradeMobile.ViewModels;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using TradeLogic;

namespace AutoTradeMobile;

public partial class TradePage : ContentPage
{
    public TradePageViewModel ViewModel { get; }

    public TradePage(TradePageViewModel viewmodel, TradeApp trade)
    {
        viewmodel.SetTradeReference(trade);
        ViewModel = viewmodel;
        InitializeComponent();
        BindingContext = ViewModel;

    }

}