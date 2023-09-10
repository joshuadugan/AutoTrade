using AutoTradeMobile.ViewModels;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Globalization;
using TradeLogic;

namespace AutoTradeMobile;

public partial class TradePage : ContentPage
{
    public TradePageViewModel ViewModel
    {
        get
        {
            return App.TradePageVM;
        }
    }

    public TradePage()
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.AccountSelected((Account)e.CurrentSelection.FirstOrDefault());
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (OrdersGrid != null)
        {
            OrdersGrid.ColumnSizer.Refresh(true);

        }

    }



}
