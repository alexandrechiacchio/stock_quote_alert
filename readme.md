# A stock monitor project

### How to use

Fill the credentials in `appsettings.json`.

Run `dotnet publish -c Release -r win-x64 --self-contained`.

Use `-r linux-x64` for Linux or `-r osx-x64` for Mac.

Run `/bin/Release/net<version>/stock_quote_alert <ticker> <buyQuote> <sellQuote>`.

