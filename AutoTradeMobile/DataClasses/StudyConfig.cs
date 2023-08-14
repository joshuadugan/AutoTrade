using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoTradeMobile
{
    public partial class StudyConfig : ObservableObject
    {
        public enum StudyType
        {
            MA,
            EMA
        }
        public enum FieldName
        {
            open,
            high,
            low,
            close
        }

        [ObservableProperty]
        StudyType type = StudyType.MA;

        [ObservableProperty]
        int period = 20;

        [ObservableProperty]
        FieldName field = FieldName.close;

        [ObservableProperty]
        bool enabledForTrading = false;

        [ObservableProperty]
        int defaultOrderSize = 100;

        [ObservableProperty]
        bool tradeOnUpTrend = true;

        [ObservableProperty]
        bool tradeOnCrossMA = false;

        [ObservableProperty]
        decimal uptrendAmountRequired = 10;


    }

}