using System.Collections.Generic;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public class SendRequest
    {
        public Address? From { get; set; }
        public Address? ReplyTo { get; set; }
        public IEnumerable<Address>? To { get; set; }
        public IEnumerable<Address>? Cc { get; set; }
        public IEnumerable<Address>? Bcc { get; set; }
        public string? Subject { get; set; }
        public string? PlainTextBody { get; set; }
        public string? HtmlBody { get; set; }
    }
}
