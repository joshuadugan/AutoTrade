using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile
{
    public partial class ChartMinute : ObservableObject
    {
        [ObservableProperty]
        decimal firstStudyValue;

        [ObservableProperty]
        decimal secondStudyValue;

        [ObservableProperty]
        string tradeMinute;

        [ObservableProperty]
        decimal open;

        [ObservableProperty]
        decimal high;

        [ObservableProperty]
        decimal low;

        [ObservableProperty]
        decimal close;

        public ChartMinute(SymbolData.Minute tm) 
        {
            this.TradeMinute = tm.TradeMinute;
            this.Open = tm.Open;
            this.High = tm.High;
            this.Low = tm.Low;
            this.Close = tm.Close;
        }

    }
}
