public class Functions
{
    public static double getQuote(string ticker)
    {
        // Get the quote for the ticker
    }
    public static void sendEmail(string subject, string body)
    {
        // Send an email
    }
    public static bool checkAlert(int n)
    {

    }
}

class MainClass
{
    static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Please enter a ticker and an interval.");
            Console.WriteLine("Usage: stock_quote_alert.exe <ticker> <buy_quote> <sell_quote> ");
            return 1;
        }

        string ticker = args[0];
        double buy_quote, sell_quote;
        if (!double.TryParse(args[1], out buy_quote) ||  !double.TryParse(args[2], out sell_quote) )
        {
            Console.WriteLine("Please enter a valid numeric value for the quotes.");
            return 1;
        }
        if(buy_quote < 0 || sell_quote < buy_quote)
        {
            Console.WriteLine("Please enter a valid value for the quotes. Buy quote should be greater than 0 and sell quote should be greater than buy quote.");
            return 1;
        }


        return 0;
    }
}