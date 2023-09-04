using CommunityToolkit.Maui.Core.Extensions;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        static Queue<PreviewOrderResponse.RequestBody> OrderRequestQueue = new Queue<PreviewOrderResponse.RequestBody>();
        static Queue<PreviewOrderResponse.RequestBody> OrderProcessingQueue = new Queue<PreviewOrderResponse.RequestBody>();
        static Queue<PreviewOrderResponse.RequestBody> CurrentPositionQueue = new Queue<PreviewOrderResponse.RequestBody>();

        private static Timer OrderTimer { get; set; }
        public bool ReplayLastSession { get; internal set; }
        public bool SimulateOrders { get; internal set; }

        static PreviewOrderResponse.RequestBody DequeOrderProcessingQueue()
        {
            if (OrderProcessingQueue.Count > 0)
            {
                return OrderProcessingQueue.Dequeue();
            }
            return null;
        }
        public static bool IsOrderPending()
        {
            return OrderRequestQueue.Any() | OrderProcessingQueue.Any();
        }

        public static void AddOrderToQueue(PreviewOrderResponse.RequestBody order)
        {
            //put it in the processing queue first to ensure no further orders will be placed until its dequeued from the processing queue and the order request queue
            OrderProcessingQueue.Enqueue(order);
            OrderRequestQueue.Enqueue(order);
        }

        private void StartOrderTimer()
        {
            if (SimulateOrders)
            {
                OrderTimer = new(OrderTimerTick_Simulated, null, 1000, 5000);
            }
            else
            {
                OrderTimer = new(OrderTimerTick, null, 1000, 5000);
            }
        }

        private void OrderTimerTick(object args)
        {
            ProcessOrderQueue();
            LoadPortfolioAsync();
            LoadOrdersAsync();
        }


        private void ProcessOrderQueue()
        {
            if (!OrderRequestQueue.Any()) { return; }

            lock (OrderTimer)
            {

                PreviewOrderResponse.RequestBody thisOrder = OrderRequestQueue.Dequeue();
                try
                {
                    PlaceOrder(thisOrder);
                }
                catch (Exception ex)
                {
                    //depending on the exception we might be able to recover.
                    //any temporary network issue we can just swallow up as we try again on the next tick.
                    ex.WriteExceptionToLog();
                }


            }
        }



    }
}
