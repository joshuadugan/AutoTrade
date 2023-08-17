using AutoTradeMobile.ViewModels;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using TradeLogic;

namespace AutoTradeMobile;

public partial class TradePage : ContentPage
{
    public TradePageViewModel ViewModel { get; }

    public string ReplayLastSession { get; set; }

    public TradePage(TradePageViewModel viewmodel, TradeApp trade)
    {
        InitializeComponent();
        ViewModel = viewmodel;
        ViewModel.ReplayLastSession = ((MainPage)Shell.Current.CurrentPage).ViewModel.ReplayLastSession;
        ViewModel.Load(trade);
        BindingContext = ViewModel;

    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.AccountSelected((Account)e.CurrentSelection.FirstOrDefault());
    }
}