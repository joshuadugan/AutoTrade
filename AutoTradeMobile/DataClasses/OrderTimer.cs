using CommunityToolkit.Maui.Core.Extensions;
using System.Diagnostics;
using TradeLogic.APIModels.Orders;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        static Queue<PreviewOrderResponse.RequestBody> OrderRequestQueue = new Queue<PreviewOrderResponse.RequestBody>();

        public static void AddOrderToQueue(PreviewOrderResponse.RequestBody order)
        {
            OrderRequestQueue.Enqueue(order);
        }

        private void StartOrderTimer(bool replayLastSession)
        {
            OrderTimer = new(ProcessOrderQueue, replayLastSession, 1000, 1000);

        }

        private Timer OrderTimer { get; set; }

        private void RequestOrderData(object args)
        {
            lock (OrderTimer)
            {
                try
                {
                    string symbol = CurrentSymbolList.First();
                    if (string.IsNullOrEmpty(AccountIdKey))
                    {
                        throw new ArgumentException("No Account Id Key");
                    }
                    Stopwatch sw = Stopwatch.StartNew();
                    OrdersListResponse OrderResult = TradeAPI.GetOrdersAsync(AccessToken, AccountIdKey, symbol).Result;
                    sw.Stop();
                    Trace.WriteLineIf(sw.Elapsed.TotalMilliseconds > 500, $"RequestOrderData delay: {sw.Elapsed.TotalMilliseconds}");
                    LastOrderResponseTime = sw.Elapsed;
                    Orders = OrderResult.Order.Select(order => new MarketOrder(order)).ToObservableCollection();
                }
                catch (Exception ex)
                {
                    TradingError = ex.Message;
                }
            }
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

        private void AddReplayOrder(PreviewOrderResponse.RequestBody thisOrder)
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(
                        () =>
                            Orders.Add(new MarketOrder(thisOrder))
                        )
                    );
            }
            else
            {
                Orders.Add(new MarketOrder(thisOrder));
            }

        }


    }
}
