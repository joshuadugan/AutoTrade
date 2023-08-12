using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TradeLogic.Authorization;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {
        internal async Task<bool> StartAuthProcessAsync()
        {
            bool OpenWindow = false;

            string AuthorizationUrl = await trader.GetAuthorizationUrlAsync();

            OpenWindow = await Browser.Default.OpenAsync(new Uri(AuthorizationUrl), BrowserLaunchMode.SystemPreferred);

            return OpenWindow;
        }

        internal async Task<bool> VerifyCodeAsync(string code)
        {
            accessToken = await trader.GetAccessToken(code);
            if (accessToken != null)
            {
                return true;
            }
            throw new Exception($"Unable to obtain access token");
        }

        internal async void StartTradingSymbolAsync(string symbol)
        {
            //lookup the symbol to see if its valid
            if (await trader.ValidateSymbolAsync(symbol))
            {
                if (!currentSymbolList.Contains(symbol))
                {
                    currentSymbolList.Add(symbol.ToUpper());
                }
                TickerTimer = new(RequestSymbolData, null, 1000, 10000);
            }
        }

        internal  SymbolData GetSymbolData(string symbol)
        {
            return Symbols.GetOrAdd(symbol.ToUpper(), new SymbolData());
        }

        internal  void StartTrading()
        {
            TickerTimer = new(RequestSymbolData, null, 1000, 10000);
        }
        internal  void StopTrading()
        {
            TickerTimer.Dispose();
        }

    }
}
