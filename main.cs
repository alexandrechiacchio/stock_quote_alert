using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StockQuoteAlert
{
    class MainClass
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Please enter a ticker and an interval.");
                Console.WriteLine("Usage: stock_quote_alert.exe <ticker> <buyQuote> <sellQuote> ");
                return 1;
            }

            string ticker = args[0];
            double buyQuote, sellQuote;
            if (!double.TryParse(args[1], out buyQuote) || !double.TryParse(args[2], out sellQuote))
            {
                Console.WriteLine("Please enter a valid numeric value for the quotes.");
                return 1;
            }
            if (buyQuote < 0 || sellQuote < buyQuote)
            {
                Console.WriteLine("Please enter a valid value for the quotes. Buy quote must be greater than 0 and sell quote greater than buy quote.");
                return 1;
            }

            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

            Console.WriteLine($"Quote monitor for ticker {ticker} set up with buy quote {buyQuote} and sell quote {sellQuote}.");


            QuoteMonitor quoteMonitor = new QuoteMonitor(ticker, buyQuote, sellQuote, config);

            var monitorRunTask = quoteMonitor.RunAsync();

            await monitorRunTask;

            return 0;
        }
    }
}