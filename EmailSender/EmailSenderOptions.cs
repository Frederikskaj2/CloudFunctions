using System.Collections.Generic;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public class EmailSenderOptions
    {
        public Dictionary<string, SmtpOptions> Senders { get; } = new Dictionary<string, SmtpOptions>();
    }
}
