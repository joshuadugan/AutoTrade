using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoTradeMobile
{
    public partial class StudyConfig : ObservableObject
    {
        public enum StudyType
        {
            SMA,
            EMA,
            VWMA //volume weighted moving average
        }
        public enum FieldName
        {
            open,
            high,
            low,
            close
        }

        [ObservableProperty]
        StudyType type = StudyType.VWMA;

        [ObservableProperty]
        int period = 20;

        [ObservableProperty]
        bool enabledForTrading = false;

        [ObservableProperty]
        int defaultOrderSize = 100;

        [ObservableProperty]
        int maxSharesInPlay = 100;

        [ObservableProperty]
        bool tradeOnMATrend = false;

        [ObservableProperty]
        bool tradeOnMinuteAverage = false;

        [ObservableProperty]
        decimal uptrendAmountRequired = 0.0m;

    }

}