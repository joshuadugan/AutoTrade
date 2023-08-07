// See https://aka.ms/new-console-template for more information
using ApiTestConsole;
using System.Diagnostics;
using System.Net.Sockets;
using TradeLogic.APIModels;
using TradeLogic.APIModels.Accounts;
using TradeLogic.Authorization;

Console.WriteLine("Starting Trading Service");

string Key = "7b7ccd8f253416219b7ba8446b6c17ba";
string Secret = "bd6175499e1eaa627121d982b901d01c4c6dd78271982fa03c6f771bd25a48d8";
bool useSandbox = false;

var Trader = new TradeLogic.Trader(Key, Secret, useSandbox);

try
{
    string authURL = Trader.GetAuthorizationUrl();

    var ps = new ProcessStartInfo(authURL)
    {
        UseShellExecute = true,
        Verb = "open"
    };

    Process.Start(ps);

}
catch (Exception ex)
{
    Console.Write(ex.Message);
    throw;
}

string? verificationKey;
do
{
    Console.Write("Enter verification key: ");

    verificationKey = Console.ReadLine();
}
while (string.IsNullOrWhiteSpace(verificationKey));



AccessToken accessToken;
try
{
    // now that we have the verification key, exchange it for the access token
    accessToken = await Trader.GetAccessToken(verificationKey);
    Console.WriteLine($"Access Token: ${accessToken.Token}");

}
catch (Exception ex)
{
    Console.Write(ex.Message);
    throw;
}


string? UserChoice;
bool doExit = false;
int index = 0;
string accountIdKey;
int secondaryChoiceIndex;
try
{
    AccountListResponse? listAccounts;
    do
    {
        Console.WriteLine("Choose Action:");
        Console.WriteLine("Press 1 to List Accounts");
        Console.WriteLine("Press 2 to Get Account Balances");
        Console.WriteLine("Press 3 to List Transactions");
        Console.WriteLine("Press 4 to View Portfolio Performance");
        Console.WriteLine("Press 5 to Get Quotes");
        Console.WriteLine("Press any other key to Exit");
        UserChoice = Console.ReadLine();
        switch (UserChoice)
        {
            case "1":

                Console.WriteLine("List Accounts:");

                listAccounts = await Trader.ListAccounts(accessToken);

                Helpers.PrintObjectProperties(listAccounts);

                break;

            case "2":

                Console.WriteLine("Account Balances:");

                listAccounts = await Trader.ListAccounts(accessToken);

                index = 0;
                foreach (Account account in listAccounts.Accounts.Account)
                {
                    index++;
                    Console.WriteLine($"Press {index} for {account.AccountId}");
                }
                while (int.TryParse(Console.ReadLine().ToString(), out secondaryChoiceIndex) == false || secondaryChoiceIndex < 1 || secondaryChoiceIndex > listAccounts.Accounts.Account.Count)
                {
                    index = 0;
                    foreach (Account account in listAccounts.Accounts.Account)
                    {
                        index++;
                        Console.WriteLine($"Press {index} for {account.AccountId}");
                    }
                }

                accountIdKey = listAccounts.Accounts.Account[secondaryChoiceIndex - 1].AccountIdKey;

                var Balances = await Trader.GetAccountBalances(accessToken, accountIdKey);

                Helpers.PrintObjectProperties(Balances);

                break;

            case "3":

                Console.WriteLine("List Transactions:");

                listAccounts = await Trader.ListAccounts(accessToken);

                index = 0;
                foreach (Account account in listAccounts.Accounts.Account)
                {
                    index++;
                    Console.WriteLine($"Press {index} for {account.AccountId}");
                }
                while (int.TryParse(Console.ReadLine().ToString(), out secondaryChoiceIndex) == false || secondaryChoiceIndex < 1 || secondaryChoiceIndex > listAccounts.Accounts.Account.Count)
                {
                    index = 0;
                    foreach (Account account in listAccounts.Accounts.Account)
                    {
                        index++;
                        Console.WriteLine($"Press {index} for {account.AccountId}");
                    }
                }

                accountIdKey = listAccounts.Accounts.Account[secondaryChoiceIndex - 1].AccountIdKey;

                var Transactions = await Trader.ListTransactions(accessToken, accountIdKey);

                Helpers.PrintObjectProperties(Transactions);


                break;

            case "4":

                Console.WriteLine("View Portfolio Performance:");

                listAccounts = await Trader.ListAccounts(accessToken);

                index = 0;
                foreach (Account account in listAccounts.Accounts.Account)
                {
                    index++;
                    Console.WriteLine($"Press {index} for {account.AccountId}");
                }
                while (int.TryParse(Console.ReadLine().ToString(), out secondaryChoiceIndex) == false || secondaryChoiceIndex < 1 || secondaryChoiceIndex > listAccounts.Accounts.Account.Count)
                {
                    index = 0;
                    foreach (Account account in listAccounts.Accounts.Account)
                    {
                        index++;
                        Console.WriteLine($"Press {index} for {account.AccountId}");
                    }
                }

                accountIdKey = listAccounts.Accounts.Account[secondaryChoiceIndex - 1].AccountIdKey;

                var Portfolio = await Trader.ViewPortfolioPerformance(accessToken, accountIdKey);

                Helpers.PrintObjectProperties(Portfolio);

                break;

            case "5":

                Console.WriteLine("Get Quotes:");
                string symbol = Console.ReadLine();

                var quote = await Trader.GetQuotes(accessToken, new List<string>() { symbol }, TradeLogic.Trader.GetQuotesDetailFlag.ALL);
                Helpers.PrintObjectProperties(quote);

                break;

            default:
                doExit = true;
                break;
        }
    }
    while (doExit == false);


}
catch (Exception ex)
{
    Console.Write(ex.Message);
    throw;
}
