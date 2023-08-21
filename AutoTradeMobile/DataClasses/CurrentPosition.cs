using CommunityToolkit.Mvvm.ComponentModel;
using TradeLogic.APIModels.Accounts.portfolio;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class CurrentPosition : ObservableObject
    {

        [ObservableProperty]
        double quantity;
        [ObservableProperty]
        double totalCost;
        [ObservableProperty]
        double costPerShare;
        [ObservableProperty]
        double marketValue;
        [ObservableProperty]
        double totalGain;
        [ObservableProperty]
        Color totalGainColor;
        [ObservableProperty]
        int responseCount;

        public void UpdateMarketValue(double LastTrade)
        {
            MarketValue = LastTrade * Quantity;
            TotalGain = MarketValue - TotalCost;
            TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
        }

        internal void MergeNewOrder(PreviewOrderResponse.RequestBody thisOrder)
        {
            // add too or sell the position
            var order = thisOrder.Order.First();
            var instrument = order.Instrument.First();

            if (instrument.OrderAction == "BUY")
            {
                var cp = new CurrentPosition(thisOrder);
                Quantity += cp.Quantity;
                TotalCost += cp.TotalCost;
                CostPerShare = TotalCost / Quantity;
                MarketValue += cp.MarketValue;
                TotalGain = MarketValue - TotalCost;
                TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
            }
            else
            {
                Quantity -= instrument.OrderedQuantity;
                CostPerShare = 0;
                TotalCost = 0;
                MarketValue = 0;
                TotalGain = 0;
                TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
            }

        }

        public CurrentPosition(int portfolioResponseCount, IGrouping<string, Position> group)
        {
            ResponseCount = portfolioResponseCount;
            Quantity = group.Sum(g => g.Quantity);
            TotalCost = group.Sum(g => g.TotalCost);
            MarketValue = group.Sum(g => g.MarketValue);
            TotalGain = group.Sum(g => g.TotalGain);
            CostPerShare = TotalCost / Quantity;
            TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
        }

        public CurrentPosition(PreviewOrderResponse.RequestBody thisOrder)
        {
            var order = thisOrder.Order.First();
            var instrument = order.Instrument.First();

            Quantity = instrument.OrderedQuantity;
            CostPerShare = order.LimitPrice;
            TotalCost = Quantity * CostPerShare;
            MarketValue = TotalCost;
            TotalGain = 0;
            TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
        }

    }
}