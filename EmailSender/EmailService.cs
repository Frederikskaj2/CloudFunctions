using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    [SuppressMessage("Microsoft.Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated by the framework.")]
    class EmailService : IEmailService
    {
        readonly SmtpClient client;
        readonly ILogger logger;
        readonly Dictionary<string, SmtpOptions> senders;

        public EmailService(ILogger<EmailService> logger, IOptions<EmailSenderOptions> options, SmtpClient client)
        {
            this.logger = logger;
            this.client = client;

            senders = options.Value.Senders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        public async Task SendAsync(SendCommand command)
        {
            var message = CreateMessage(command);
            var email = command.From!.Email!;
            await SendAsync(email, message);
            LogMessage(message);
        }

        async Task SendAsync(string email, MimeMessage message)
        {
            var options = senders[email];
            await client.ConnectAsync(options.ServerName, options.Port, options.SocketOptions);
            await client.AuthenticateAsync(email, options.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        void LogMessage(MimeMessage message)
        {
            using var stream = new MemoryStream();
            message.WriteTo(stream, true);
            logger.LogInformation("Mail sent\n" + Encoding.UTF8.GetString(stream.ToArray()));
        }

        static MimeMessage CreateMessage(SendCommand command)
        {
            var message = new MimeMessage();

            message.From.Add(CreateAddress(command.From!));

            if (command.ReplyTo != null)
                message.ReplyTo.Add(CreateAddress(command.ReplyTo));

            foreach (var to in command.To!)
                message.To.Add(CreateAddress(to));

            if (command.Cc != null)
                foreach (var cc in command.Cc!)
                    message.Cc.Add(CreateAddress(cc));

            if (command.Bcc != null)
                foreach (var bcc in command.Bcc!)
                    message.Bcc.Add(CreateAddress(bcc));

            message.Subject = command.Subject;

            var bodyBuilder = new BodyBuilder();
            if (!string.IsNullOrEmpty(command.PlainTextBody))
                bodyBuilder.TextBody = command.PlainTextBody;
            if (!string.IsNullOrEmpty(command.HtmlBody))
                bodyBuilder.HtmlBody = command.HtmlBody;
            message.Body = bodyBuilder.ToMessageBody();
            return message;
        }

        static MailboxAddress CreateAddress(Address address) => new MailboxAddress(address.Name, address.Email);
    }
}
