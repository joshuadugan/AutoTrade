using CommunityToolkit.Maui.Core.Extensions;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        static Queue<PreviewOrderResponse.RequestBody> OrderRequestQueue = new Queue<PreviewOrderResponse.RequestBody>();
        static Queue<PreviewOrderResponse.RequestBody> CurrentPositionQueue = new Queue<PreviewOrderResponse.RequestBody>();

        private static Timer OrderTimer { get; set; }

        public static void AddOrderToQueue(PreviewOrderResponse.RequestBody order)
        {
            OrderRequestQueue.Enqueue(order);
        }

        private void StartOrderTimer(bool replayLastSession)
        {
            OrderTimer = new(OrderTimerTick, replayLastSession, 1000, 5000);
        }

        private void OrderTimerTick(object replayLastSession)
        {
            ProcessOrderQueue((bool)replayLastSession);
            LoadPortfolioAsync((bool)replayLastSession);
            LoadOrdersAsync((bool)replayLastSession);
        }


        private void ProcessOrderQueue(object args)
        {
            lock (OrderTimer)
            {
                if (OrderRequestQueue.Count > 0)
                {
                    PreviewOrderResponse.RequestBody thisOrder = OrderRequestQueue.Dequeue();

                    if ((bool)args)
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
            Orders.Add(new MarketOrder(thisOrder));

            CurrentPositionQueue.Enqueue(thisOrder);
        }

    }
}
