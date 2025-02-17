using System.Text.Json;
using System.Xml.Schema;
using Microsoft.Extensions.Configuration;

namespace StockQuoteAlert
{

    public class QuoteMonitor
    {
        private readonly string ticker;
        private readonly double buy_quote;
        private readonly double sell_quote;
        private double cur_quote;
        private double last_quote;
        private readonly string token;
        private readonly Email email;

        public QuoteMonitor(string _ticker, double _buyQuote, double _sellQuote, IConfiguration? _config = null)
        {
            ticker = _ticker;
            buy_quote = _buyQuote;
            sell_quote = _sellQuote;
            email = new Email();

            _config ??= new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

            token = _config.GetValue<string>("apiKey") ?? throw new ArgumentNullException(nameof(_config), "Config file must contain apiKey");

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
                Console.Write($"\rQuote of {ticker} at {DateTime.Now}: R${quote}");

                return cur_quote = quote;
            }
        }

        public void CheckQuote()
        {
            if (cur_quote < buy_quote && last_quote > buy_quote)
            {
                Console.WriteLine($"\nBuy {ticker} at {cur_quote}");
                email.SendEmail($"Buy {ticker} at {cur_quote}", $"Buy rule for ticker {ticker} at buy quote {buy_quote} was triggered. Current quote is {cur_quote}");
                return;
            }
            if (cur_quote > sell_quote && last_quote < sell_quote)
            {
                Console.WriteLine($"\nSell {ticker} at {cur_quote}");
                email.SendEmail($"Sell {ticker} at {cur_quote}", $"Sell rule for ticker {ticker} at sell quote {sell_quote} was triggered. Current quote is {cur_quote}");
                return;
            }
        }

        public async Task RunAsync()
        {
            await SetQuoteAsync(); // set cur_quote
            while (true)
            {
                last_quote = cur_quote;
                await SetQuoteAsync();
                CheckQuote();
                await Task.Delay(60000); // larger than 1 second so I dont finish up the API limit
                // the API free plan is so bad that it only updates every 30 minutes
            }
        }
    }
}