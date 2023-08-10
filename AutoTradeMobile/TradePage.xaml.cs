using TradeLogic;

namespace AutoTradeMobile;

public partial class TradePage : ContentPage
{
    public TradePage()
    {
        InitializeComponent();
    }

    internal void HandleError(Exception ex)
    {
        lblError.Text = ex.Message;
        lblError.IsVisible = true;
    }
    internal void ClearError()
    {
        lblError.Text = string.Empty;
        lblError.IsVisible = false;
    }

    internal SymbolData PageData { get; set; }

    private void btnTicker_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtSymbol.Text)) { throw new Exception("No Symbol"); }

        try
        {

            Title = txtSymbol.Text;
            txtSymbol.IsVisible = false;
            btnTicker.IsVisible = false;
            //startup the trader with this symbol
            TradeApp.StartTradingSymbolAsync(txtSymbol.Text);
            //get the trade data object reference for the page context
            PageData = TradeApp.GetSymbolData(txtSymbol.Text);

        }
        catch (Exception ex)
        {
            TradeApp.StopTrading();
            HandleError(ex);
        }

    }


}