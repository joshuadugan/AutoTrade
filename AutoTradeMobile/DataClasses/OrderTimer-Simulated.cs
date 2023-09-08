using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {

        private static void OrderTimerTick_Simulated(object args)
        {
            lock (OrderTimer)
            {
                ProcessSendOrderQueue_Simulated();
                TestForOrderFill_Simulated();
                LoadPortfolioAsync_Simulated();
            }
        }

        private static void ProcessSendOrderQueue_Simulated()
        {
            if (!SendOrderQueue.Any()) { return; }

            PreviewOrderResponse.RequestBody thisOrder = SendOrderQueue.Peek();
            PendingFillQueue.Enqueue(thisOrder);
            SendOrderQueue.Dequeue();//processed - remove
        }

        private static void TestForOrderFill_Simulated()
        {
            if (!PendingFillQueue.Any()) { return; }

            PreviewOrderResponse.RequestBody thisOrder = PendingFillQueue.Peek();

            bool fillOrder = false;
            var orderDetail = thisOrder.Order.First();
            if (orderDetail.Instrument.First().OrderAction == "BUY")
            {
                fillOrder = true;
            }
            else if (orderDetail.Instrument.First().OrderAction == "SELL")
            {
                if (orderDetail.PeakPrice < Symbol.LastTrade)
                {
                    orderDetail.PeakPrice = Symbol.LastTrade;
                }
                var StopPrice = orderDetail.PeakPrice - orderDetail.OffsetValue;
                if (Symbol.LastTrade < StopPrice)
                {
                    orderDetail.LimitPrice = StopPrice;
                    fillOrder = true;
                }
            }

            if (fillOrder)
            {
                FillOrder(thisOrder);
                CurrentPositionQueue.Enqueue(thisOrder);
                DequeOrderProcessingQueue();
            }
        }

        private static void FillOrder(PreviewOrderResponse.RequestBody thisOrder)
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(
                        () =>
                        {
                            //simulate the order being filled
                            TradeApp.OrderState.Orders.Add(new MarketOrder(thisOrder));
                        })
                    );
            }
        }

        private static void LoadPortfolioAsync_Simulated()
        {
            //process items in the que to simulate getting new portfolio data
            if (CurrentPositionQueue.Count > 0)
            {
                var pos = CurrentPositionQueue.Dequeue();
                Symbol.CurrentPosition.MergeNewOrder(pos);
            }
        }

    }
}
