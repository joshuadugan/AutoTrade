using AutoTradeMobile.ViewModels;

namespace AutoTradeMobile;

public partial class SettingsPage : ContentPage
{

    public TradePageViewModel ViewModel
    {
        get
        {
            return App.TradePageVM;
        }
    }

    public SettingsPage()
	{
		InitializeComponent();
        BindingContext = ViewModel;

    }
}