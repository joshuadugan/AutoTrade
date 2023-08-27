using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using TradeLogic.APIModels.Accounts.portfolio;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class CurrentPosition : ObservableObject
    {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HaveShares))]
        decimal quantity;

        public bool HaveShares
        {
            get
            {
                return Quantity > 0;
            }
        }

        [ObservableProperty]
        decimal totalCost;

        [ObservableProperty]
        decimal costPerShare;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TrailingValue))]
        [NotifyPropertyChangedFor(nameof(TrailingValueColor))]
        decimal marketValue;

        [ObservableProperty]
        decimal totalGain;

        [ObservableProperty]
        Color totalGainColor;

        [ObservableProperty]
        int responseCount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TrailingStopPrice))]
        decimal highSharePrice;

        public decimal TrailingStopPrice
        {
            get
            {
                return HighSharePrice - StopAmount;
            }
        }

        public decimal TrailingValue
        {
            get
            {
                return Quantity * TrailingStopPrice - TotalCost;
            }
        }

        public Color TrailingValueColor
        {
            get
            {
                return TrailingValue >= 0 ? Colors.Green : Colors.Red;
            }
        }

        [ObservableProperty]
        decimal stopAmount = .5m;

        public void UpdateMarketValue(decimal LastTrade)
        {
            if (LastTrade > HighSharePrice)
            {
                HighSharePrice = LastTrade;
            }
            MarketValue = LastTrade * Quantity;
            TotalGain = MarketValue - TotalCost;
            TotalGainColor = TotalGain >= 0 ? Colors.Green : Colors.Red;
        }

        internal void MergeNewOrder(PreviewOrderResponse.RequestBody thisOrder)
        {
            StopAmount = TradeApp.SymbolData.VelocityTradeTrailingStopValue;
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
                HighSharePrice = OrderLimitPrice;
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