using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

using Microsoft.Extensions.Configuration;

namespace email
{
    public class EmailSettings
    {
        public Dictionary<string, string> smtpConfigs { get; set; }
        // public List<Dictionary<string, string>> recipientsList { get; set; }
        public string fromEmail { get; set; }
        public string fromName { get; set; }
        public string toEmail { get; set; }
        public string toName { get; set; }

        public string subject { get; set; }

        public string body { get; set; }

    }
    public class Email
    {
        private readonly EmailSettings _emailSettings;

        public Email()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _emailSettings = new EmailSettings();
            config.GetSection("emailConfigs").Bind(_emailSettings);

            Console.WriteLine("SMTP Configurations:");
            foreach (var i in _emailSettings.smtpConfigs)
            {
                Console.WriteLine($"{i.Key}: {i.Value}");
            }

            Console.WriteLine($"\nTo: {_emailSettings.toEmail}");
            Console.WriteLine($"To Name: {_emailSettings.toName}");

            Console.WriteLine($"\nFrom: {_emailSettings.fromEmail}");
            Console.WriteLine($"From Name: {_emailSettings.fromName}");
            Console.WriteLine($"Subject: {_emailSettings.subject}");
            Console.WriteLine($"Body: {_emailSettings.body}");

        }

        public void sendEmail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.fromName, _emailSettings.fromEmail));
            message.To.Add(new MailboxAddress(_emailSettings.toName, _emailSettings.toEmail));
            message.Subject = _emailSettings.subject;

            message.Body = new TextPart("plain")
            {
                Text = _emailSettings.body
            };

            using (var client = new SmtpClient())
            {

                client.Connect(_emailSettings.smtpConfigs["server"], int.Parse(_emailSettings.smtpConfigs["port"]), true);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_emailSettings.smtpConfigs["username"], _emailSettings.smtpConfigs["password"]);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}