using CommunityToolkit.Maui.Core.Extensions;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        static Queue<PreviewOrderResponse.RequestBody> SendOrderQueue = new Queue<PreviewOrderResponse.RequestBody>();
        static Queue<PreviewOrderResponse.RequestBody> PendingFillQueue = new Queue<PreviewOrderResponse.RequestBody>();
        static Queue<PreviewOrderResponse.RequestBody> CurrentPositionQueue = new Queue<PreviewOrderResponse.RequestBody>();

        private static Timer OrderTimer { get; set; }

        static PreviewOrderResponse.RequestBody DequeOrderProcessingQueue()
        {
            if (PendingFillQueue.Count > 0)
            {
                return PendingFillQueue.Dequeue();
            }
            return null;
        }
        public static bool IsOrderPending()
        {
            return SendOrderQueue.Any() | PendingFillQueue.Any();
        }

        public static void AddOrderToQueue(PreviewOrderResponse.RequestBody order)
        {
            SendOrderQueue.Enqueue(order);
        }

        private void StartOrderTimer()
        {
            if (SimulateOrders)
            {
                OrderTimer = new(OrderTimerTick_Simulated, null, 1000, 500);
            }
            else
            {
                OrderTimer = new(OrderTimerTick, null, 1000, 1000);
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
            if (!SendOrderQueue.Any()) { return; }

            lock (OrderTimer)
            {

                PreviewOrderResponse.RequestBody thisOrder = SendOrderQueue.Dequeue();
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
