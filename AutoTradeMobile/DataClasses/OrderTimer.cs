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

        private static Timer OrderTimer { get; set; }
        public bool ReplayLastSession { get; internal set; }
        public bool SimulateOrders { get; internal set; }

        public static void AddOrderToQueue(PreviewOrderResponse.RequestBody order)
        {
            //put it in the processing queue first to ensure no further orders will be placed until its dequeued from the processing queue and the order request queue
            OrderProcessingQueue.Enqueue(order);
            OrderRequestQueue.Enqueue(order);
        }

        private void StartOrderTimer()
        {
            OrderTimer = new(OrderTimerTick, null, 1000, 5000);
        }

        private void OrderTimerTick(object args)
        {
            ProcessOrderQueue();
            LoadPortfolioAsync();
            LoadOrdersAsync();
        }


        private void ProcessOrderQueue()
        {
            lock (OrderTimer)
            {
                if (OrderRequestQueue.Count > 0)
                {
                    PreviewOrderResponse.RequestBody thisOrder = OrderRequestQueue.Dequeue();

                    if ((bool)SimulateOrders)
                    {
                        //process the order virtually
                        AddReplayOrder(thisOrder);
                    }
                }
            }
        }

        //private void RequestOrderData(object args)
        //{
        //    lock (OrderTimer)
        //    {
        //        try
        //        {
        //            string symbol = CurrentSymbolList.First();
        //            if (string.IsNullOrEmpty(AccountIdKey))
        //            {
        //                throw new ArgumentException("No Account Id Key");
        //            }
        //            Stopwatch sw = Stopwatch.StartNew();
        //            OrdersListResponse OrderResult = TradeAPI.GetOrdersAsync(AccessToken, AccountIdKey, symbol).Result;
        //            sw.Stop();
        //            Trace.WriteLineIf(sw.Elapsed.TotalMilliseconds > 500, $"RequestOrderData delay: {sw.Elapsed.TotalMilliseconds}");
        //            LastOrderResponseTime = sw.Elapsed;
        //            Orders = OrderResult.Order.Select(order => new MarketOrder(order)).ToObservableCollection();
        //        }
        //        catch (Exception ex)
        //        {
        //            TradingError = ex.Message;
        //        }
        //    }
        //}


        private void AddReplayOrder(PreviewOrderResponse.RequestBody thisOrder)
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(
                        () => AddOrderAndPosition(thisOrder))
                    );
            }
            else
            {
                AddOrderAndPosition(thisOrder);
            }

        }

        private void AddOrderAndPosition(PreviewOrderResponse.RequestBody thisOrder)
        {
            //simulate the order being placed and accepted
            Orders.Add(new MarketOrder(thisOrder));
            //simulate the order bing filled
            CurrentPositionQueue.Enqueue(thisOrder);
        }

    }
}
