
using email;
class MainClass
{
    static int Main(string[] args)
    {
        // if (args.Length < 3)
        // {
        //     Console.WriteLine("Please enter a ticker and an interval.");
        //     Console.WriteLine("Usage: stock_quote_alert.exe <ticker> <buy_quote> <sell_quote> ");
        //     return 1;
        // }

        // string ticker = args[0];
        // double buy_quote, sell_quote;
        // if (!double.TryParse(args[1], out buy_quote) ||  !double.TryParse(args[2], out sell_quote) )
        // {
        //     Console.WriteLine("Please enter a valid numeric value for the quotes.");
        //     return 1;
        // }
        // if(buy_quote < 0 || sell_quote < buy_quote)
        // {
        //     Console.WriteLine("Please enter a valid value for the quotes. Buy quote should be greater than 0 and sell quote should be greater than buy quote.");
        //     return 1;
        // }

        // string ticker = "PETR4";
        // double buy_quote = 20.0;
        // double sell_quote = 25.0;

        // Console.WriteLine($"Ticker: {ticker}");
        // Console.WriteLine($"Buy Quote: {buy_quote}");
        // Console.WriteLine($"Sell Quote: {sell_quote}");


        // QuoteMonitor quoteMonitor = new QuoteMonitor(ticker, buy_quote, sell_quote);
        // double quote = quoteMonitor.SetQuoteAsync().Result;
        // Console.WriteLine($"Current quote of {ticker}: {quote}");

        // var monitorRunTask = quoteMonitor.Run();

        // Task.WhenAll(monitorRunTask).Wait(); // change this to A list later

        var email = new Email();

        email.sendEmail();

        return 0;
    }
}