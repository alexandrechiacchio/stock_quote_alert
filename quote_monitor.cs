using System.Text.Json;


public class QuoteMonitor
{
    private readonly string  ticker;
    private readonly double buy_quote;
    private readonly double sell_quote;
    private double cur_quote;
    private double last_quote;
    private readonly string token;
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
            Console.WriteLine($"Quote of {ticker} at {DateTime.Now}: R${quote}");

            return cur_quote = quote;
        }
    }

    public void CheckQuote()
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
            CheckQuote();
            last_quote = cur_quote;
            Task.Delay(1000).Wait(); // 172,8 seconds so I dont finish up the API limit
            // the API is so bad that the free plan only updates every 30 minutes

        }
    }
}

