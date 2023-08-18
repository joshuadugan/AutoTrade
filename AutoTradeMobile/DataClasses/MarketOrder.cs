using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class MarketOrder : ObservableObject
    {
        public MarketOrder(Order order)
        {
            Update(order);
        }

        [ObservableProperty]
        int orderId;

        [ObservableProperty]
        string orderType;

        [ObservableProperty]
        string details;

        public double FilledQuantity
        {
            get
            {
                return OrderResponse?.OrderDetail?.Instrument?.Sum(i => i.FilledQuantity) ?? 0; 
            }
        }

        public double OrderedQuantity
        {
            get
            {
                return OrderResponse?.OrderDetail?.Instrument?.Sum(i => i.OrderedQuantity) ?? 0;
            }
        }

        public double OrderValue
        {
            get
            {
                return OrderResponse?.OrderDetail?.OrderValue ?? 0;
            }
        }


        public Order OrderResponse { get; private set; }

        internal MarketOrder Update(Order order)
        {
            if (order != null)
            {
                //pull out the important properties and store the rest
                this.OrderId = order.OrderId;
                this.OrderType = order.OrderType;
                this.Details = order.Details;

                this.OrderResponse = order;

            }

            return this;
        }
    }
}
