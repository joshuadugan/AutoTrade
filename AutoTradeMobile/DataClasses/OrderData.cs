using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile
{
    public partial class OrderData : ObservableObject
    {
        public OrderData()
        {
            orders.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TodayProfit));
                OnPropertyChanged(nameof(TodayProfitColor));
            };

        }

        [ObservableProperty]
        ObservableCollection<MarketOrder> orders = new();

        public Color TodayProfitColor
        {
            get
            {
                return TodayProfit >= 0 ? Colors.LightGreen : Colors.Red;
            }
        }

        public decimal TodayProfit
        {
            get
            {
                return TodayOrderSells - TodayOrderBuys;
            }
        }

        private List<MarketOrder> GetVisibleOrders()
        {
            var orders = Orders.ToList();
            if (orders.Count() % 2 != 0)
            {
                orders = orders.Take(orders.Count() - 1).ToList();
            }
            return orders;
        }

        public decimal TodayOrderBuys
        {
            get
            {
                return GetVisibleOrders().Where(o => o.OrderAction == MarketOrder.OrderActions.BUY).Sum(o => o.OrderValue);
            }
        }

        public decimal TodayOrderSells
        {
            get
            {
                return GetVisibleOrders().Where(o => o.OrderAction == MarketOrder.OrderActions.SELL).Sum(o => o.OrderValue);
            }
        }

    }
}
