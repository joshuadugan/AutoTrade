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

        private void OrderTimerTick_Simulated(object args)
        {
            ProcessOrderQueue_Simulated();
            LoadPortfolioAsync_Simulated();
        }

        private void ProcessOrderQueue_Simulated()
        {
            lock (OrderTimer)
            {
                if (!OrderRequestQueue.Any()) { return; }

                PreviewOrderResponse.RequestBody thisOrder = OrderRequestQueue.Peek();
                if (thisOrder != null)
                {
                    //process the order virtually
                    AddReplayOrder_Simulated(thisOrder);
                    OrderRequestQueue.Dequeue();//processed - remove
                }
            }
        }

        private void AddReplayOrder_Simulated(PreviewOrderResponse.RequestBody thisOrder)
        {
            if (Application.Current.Dispatcher.IsDispatchRequired)
            {
                Application.Current.Dispatcher.Dispatch((Action)(
                        () => AddOrderAndPosition_Simulated(thisOrder))
                    );
            }
            else
            {
                AddOrderAndPosition_Simulated(thisOrder);
            }

        }

        private void AddOrderAndPosition_Simulated(PreviewOrderResponse.RequestBody thisOrder)
        {
            //simulate the order being placed and accepted
            Orders.Add(new MarketOrder(thisOrder));
            //simulate the order bing filled
            CurrentPositionQueue.Enqueue(thisOrder);
        }

        internal void LoadPortfolioAsync_Simulated()
        {
            //process items in the que to simulate getting new portfolio data
            if (CurrentPositionQueue.Count > 0)
            {
                var pos = CurrentPositionQueue.Dequeue();
                SymbolData.CurrentPosition.MergeNewOrder(pos);
                DequeOrderProcessingQueue();
            }

        }

    }
}
