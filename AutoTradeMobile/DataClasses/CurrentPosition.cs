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
            var OrderQuantity = instrument.OrderedQuantity;
            var OrderLimitPrice = order.LimitPrice;
            var OrderTotalCost = OrderQuantity * OrderLimitPrice;

            if (instrument.OrderAction == "BUY")
            {
                Quantity += OrderQuantity;
                TotalCost += OrderTotalCost;
                CostPerShare = TotalCost / Quantity;
                MarketValue += OrderTotalCost;
                TotalGain += MarketValue - TotalCost;
                TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
            }
            else
            {
                //sell profit
                var profit = (OrderTotalCost) - TotalCost;
                Quantity -= OrderQuantity;
                CostPerShare = OrderLimitPrice;
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