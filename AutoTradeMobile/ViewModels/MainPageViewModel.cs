using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoTradeMobile.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {

        [RelayCommand]
        public async Task Load()
        {
            await Shell.Current.GoToAsync("TradeTabPage");
        }

    }
}
