using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace StockQuoteAlert
{
    public class EmailSettings
    {
        public Dictionary<string, string> SmtpConfigs { get; set; } = [];
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
        public string ToName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;


    }
    public class Email
    {
        private readonly EmailSettings _emailSettings;

        public Email(IConfigurationRoot? config = null)
        {
            config ??= new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

            _emailSettings = new EmailSettings();
            config.GetSection("emailConfigs").Bind(_emailSettings);
        }

        public void DebugSendEmail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress(_emailSettings.ToName, _emailSettings.ToEmail));
            message.Subject = _emailSettings.Subject;

            message.Body = new TextPart("plain")
            {
                Text = _emailSettings.Body
            };

            using (var client = new SmtpClient())
            {

                client.Connect(_emailSettings.SmtpConfigs["server"], int.Parse(_emailSettings.SmtpConfigs["port"]), true);

                client.Authenticate(_emailSettings.SmtpConfigs["username"], _emailSettings.SmtpConfigs["password"]);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void SendEmail(string _subject, string _body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress(_emailSettings.ToName, _emailSettings.ToEmail));
            message.Subject = _subject;

            message.Body = new TextPart("plain")
            {
                Text = _body
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailSettings.SmtpConfigs["server"], int.Parse(_emailSettings.SmtpConfigs["port"]), true);

                    client.Authenticate(_emailSettings.SmtpConfigs["username"], _emailSettings.SmtpConfigs["password"]);

                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    throw;
                }
            }
        }
    }
}