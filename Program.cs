using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading.Tasks;


public class QuoteMonitor
{
    private string ticker;
    private double buy_quote;
    private double sell_quote;
    private double cur_quote;
    private double last_quote;
    private string token;
    public QuoteMonitor(string ticker, double buy_quote, double sell_quote)
    {
        this.ticker = ticker;
        this.buy_quote = buy_quote;
        this.sell_quote = sell_quote;
        this.token = File.ReadAllText("/home/chiacchio/inoa/ps/stock_quote_alert/apitoken"); // change this hardcode to something else later
        SetQuoteAsync().Wait();
    }
    /// <summary>
    ///  Set cur_quote to the current quote for the ticker
    /// </summary>
    /// <returns>Quote value</returns>
    public async Task<double> SetQuoteAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            var url = $"https://brapi.dev/api/quote/{ticker}";
            var query = new Dictionary<string, string>
            {
                // { "range", "1d" },
                // { "interval", "1m" },
                // { "fundamental", "false" },
                // { "dividends", "false" },
                // { "modules", "balanceSheetHistory" },
                { "token", token }
            };

            var uriBuilder = new UriBuilder(url);
            var queryString = new FormUrlEncodedContent(query).ReadAsStringAsync().Result;
            uriBuilder.Query = queryString;

            HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);
            string responseBody = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message} \n" + responseBody);
                throw;
            }

            // Parse the JSON response
            double quote = JsonDocument.Parse(responseBody).RootElement.GetProperty("results")[0].GetProperty("regularMarketPrice").GetDouble();

            return cur_quote = quote;
        }
    }


    // public static void sendEmail(string subject, string body)
    // {
    //     // Send an email
    // }

    public async Task CheckQuoteAsync()
    {
        if (cur_quote <= buy_quote && last_quote > buy_quote)
        {
            Console.WriteLine($"Buy {ticker} at {cur_quote}");
            // sendEmail($"Buy {ticker} at {cur_quote}", $"Buy {ticker} at {cur_quote}");
            return;
        }
        if (cur_quote >= sell_quote && last_quote < sell_quote)
        {
            Console.WriteLine($"Sell {ticker} at {cur_quote}");
            // sendEmail($"Sell {ticker} at {quote}", $"Sell {ticker} at {quote}");
            return;
        }
    }

    public async Task Run()
    {
        while (true)
        {
            await SetQuoteAsync();
            await CheckQuoteAsync();
            last_quote = cur_quote;
            Task.Delay(1728000).Wait(); // 172,8 seconds so I dont finish up the API limit
            // the API is so bad that the free plan only updates every 30 minutes

        }
    }
}

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

        string ticker = "PE4";
        double buy_quote = 20.0;
        double sell_quote = 25.0;

        Console.WriteLine($"Ticker: {ticker}");
        Console.WriteLine($"Buy Quote: {buy_quote}");
        Console.WriteLine($"Sell Quote: {sell_quote}");


        QuoteMonitor quoteMonitor = new QuoteMonitor(ticker, buy_quote, sell_quote);
        double quote = quoteMonitor.SetQuoteAsync().Result;
        Console.WriteLine($"Current quote of {ticker}: {quote}");

        var monitorRunTask = quoteMonitor.Run();

        Task.WhenAll(monitorRunTask).Wait(); // change this to A list later

        return 0;
    }
}