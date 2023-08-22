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
                Quantity += instrument.OrderedQuantity;
                TotalCost += instrument.OrderedQuantity * order.LimitPrice;
                CostPerShare = TotalCost / Quantity;
                MarketValue += instrument.OrderedQuantity * order.LimitPrice;
                TotalGain += MarketValue - TotalCost;
                TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
            }
            else
            {
                //sell profit
                var profit = (instrument.OrderedQuantity * order.LimitPrice) - TotalCost;
                Quantity -= instrument.OrderedQuantity;
                CostPerShare = order.LimitPrice;
                TotalCost = 0;
                MarketValue = 0;
                TotalGain += profit;
                TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
            }

        }

        //for use in real trading loaded from api
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

        public CurrentPosition()
        {
        }
    }
}