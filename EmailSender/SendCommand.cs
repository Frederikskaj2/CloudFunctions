using System.Collections.Generic;
using LanguageExt;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public record SendCommand(
        Address From,
        Address? ReplyTo,
        IEnumerable<Address> To,
        IEnumerable<Address> Cc,
        IEnumerable<Address> Bcc,
        string? Subject,
        string? PlainTextBody,
        string? HtmlBody)
    {
        public static SendCommand Create(
            Address from,
            Option<Address> replyTo,
            IEnumerable<Address> to,
            IEnumerable<Address> cc,
            IEnumerable<Address> bcc,
            Option<string> subject,
            Option<string> plainTextBody,
            Option<string> htmlBody) =>
            new SendCommand(
                from,
                replyTo.NullIfNone(),
                to,
                cc,
                bcc,
                subject.NullIfNone(),
                plainTextBody.NullIfNone(),
                htmlBody.NullIfNone());
    }
}
