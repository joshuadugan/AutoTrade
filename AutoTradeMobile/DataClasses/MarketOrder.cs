using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.Licensing;
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

        private static int simulatedOrderId = 1;
        /// <summary>
        /// This exists to simulate an order
        /// </summary>
        /// <param name="thisOrder"></param>
        public MarketOrder(PreviewOrderResponse.RequestBody thisOrder)
        {
            //make up an order id
            OrderId = simulatedOrderId++;
            OrderType = "SIMULATED";
            var order = thisOrder.Order.First();
            var instrument = order.Instrument.First();
            Details = instrument.OrderAction.ToString();
            OrderAction = Enum.Parse<OrderActions>(instrument.OrderAction);
            thisOrder.Order.First().Instrument.First().FilledQuantity = instrument.OrderedQuantity;
            thisOrder.Order.First().OrderValue = instrument.OrderedQuantity * order.LimitPrice;
            OrderResponse = new Order()
            {
                OrderDetail = thisOrder.Order,
            };
        }

        [ObservableProperty]
        int orderId;

        [ObservableProperty]
        string orderType;

        [ObservableProperty]
        string details;


        public decimal FilledQuantity
        {
            get
            {
                return OrderResponse?.OrderDetail.First().Instrument?.Sum(i => i.FilledQuantity) ?? 0;
            }
        }

        public decimal OrderedQuantity
        {
            get
            {
                return OrderResponse?.OrderDetail.First().Instrument?.Sum(i => i.OrderedQuantity) ?? 0;
            }
        }

        public decimal OrderValue
        {
            get
            {
                return OrderResponse?.OrderDetail.First().OrderValue ?? 0;
            }
        }

        public OrderActions OrderAction { get; set; }
        public enum OrderActions
        {
            BUY, SELL
        }

        public Order OrderResponse { get; private set; }

        internal MarketOrder Update(Order order)
        {
            if (order != null)
            {
                //pull out the important properties and store the rest
                this.OrderId = order.OrderId;
                this.OrderType = order.OrderType;
                this.Details = order.OrderDetail.First().Status;
                this.OrderAction = Enum.Parse<OrderActions>(order.OrderDetail.First().Status);
                this.OrderResponse = order;

            }

            return this;
        }

    }
}
