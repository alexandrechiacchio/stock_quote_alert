using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;


public class QuoteMonitor
{
    private string ticker;
    private double buy_quote;
    private double sell_quote;

    private string token;
    public QuoteMonitor(string ticker, double buy_quote, double sell_quote)
    {
        this.ticker = ticker;
        this.buy_quote = buy_quote;
        this.sell_quote = sell_quote;
        this.token = File.ReadAllText("apitoken");
    }

    public async Task<double?> GetQuoteAsync()
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
            try {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message} \n" + responseBody);
                return null;
            }

            // Parse the JSON response
            double quote = JsonDocument.Parse(responseBody).RootElement.GetProperty("results")[0].GetProperty("regularMarketPrice").GetDouble();

            return quote;
        }
    }


    // public static void sendEmail(string subject, string body)
    // {
    //     // Send an email
    // }
    // public static bool checkAlert(int n)
    // {
    //     // checks the interval
    // }
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

        string ticker = "PETR4";
        double buy_quote = 20.0;
        double sell_quote = 25.0;

        Console.WriteLine($"Ticker: {ticker}");
        Console.WriteLine($"Buy Quote: {buy_quote}");
        Console.WriteLine($"Sell Quote: {sell_quote}");


        QuoteMonitor quoteMonitor = new QuoteMonitor(ticker, buy_quote, sell_quote);
        double quote = quoteMonitor.GetQuoteAsync().Result ?? throw new Exception("Failed to get quote");
        Console.WriteLine($"Quote: {quote}");


        return 0;
    }
}