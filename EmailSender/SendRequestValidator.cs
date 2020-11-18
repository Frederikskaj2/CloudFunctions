using LanguageExt;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static LanguageExt.Prelude;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    [SuppressMessage("Microsoft.Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated by the framework.")]
    class SendRequestValidator : ISendRequestValidator
    {
        readonly ISet<string> validSenders;

        public SendRequestValidator(IOptions<EmailSenderOptions> options) =>
            validSenders = options.Value.Senders.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

        public Validation<string, SendCommand> Validate(SendRequest request) => Validate(IsValidSender, request);

        bool IsValidSender(string email) => validSenders.Contains(email);

        static Validation<string, SendCommand> Validate(Func<string, bool> isValidSender, SendRequest request)
        {
            var from = ValidateFrom(isValidSender, request.From);
            var replyTo = ValidateAddressOptional(request.ReplyTo, "Reply to");
            var receivers = ValidateReceivers(request.To, request.Cc, request.Bcc);
            // Bang required to remove compiler warning. Null is implicitly converted to None.
            var subject = Success<string, Option<string>>(request.Subject!);
            var body = ValidateBody(request.PlainTextBody, request.HtmlBody);
            return (from, replyTo, receivers, subject, body).Apply(CreateCommand);

            static SendCommand CreateCommand(
                Address from,
                Option<Address> replyTo,
                (IEnumerable<Address> To, IEnumerable<Address> Cc, IEnumerable<Address> Bcc) receivers,
                Option<string> subject,
                (Option<string> PlainText, Option<string> Html) body) =>
                SendCommand.Create(from, replyTo, receivers.To, receivers.Cc, receivers.Bcc, subject, body.PlainText, body.Html);
        }

        static Validation<string, Address> ValidateFrom(Func<string, bool> isValidSender, Address? @from) =>
            from fromNotNull in ValidateAddress(@from, "From")
            from _ in ValidateSender(isValidSender, fromNotNull.Email!)
            select fromNotNull;

        static Validation<string, (IEnumerable<Address> To, IEnumerable<Address> Cc, IEnumerable<Address> Bcc)> ValidateReceivers(IEnumerable<Address>? to, IEnumerable<Address>? cc, IEnumerable<Address>? bcc) =>
            from toNotNull in ValidateAddressesOptional(to, "To")
            from ccNotNull in ValidateAddressesOptional(cc, "CC")
            from bccNotNull in ValidateAddressesOptional(bcc, "BCC")
            from receivers in ValidateAtLeastOneReceiver(toNotNull, ccNotNull, bccNotNull)
            select receivers;

        static Validation<string, (IEnumerable<Address> To, IEnumerable<Address> Cc, IEnumerable<Address> Bcc)> ValidateAtLeastOneReceiver(IEnumerable<Address> to, IEnumerable<Address> cc, IEnumerable<Address> bcc) =>
            to.Concat(cc).Concat(bcc).Any()
                ? Success<string, (IEnumerable<Address> To, IEnumerable<Address> Cc, IEnumerable<Address> Bcc)>((to, cc, bcc))
                : Fail<string, (IEnumerable<Address> To, IEnumerable<Address> Cc, IEnumerable<Address> Bcc)>("No recievers.");

        static Validation<string, (Option<string> PlainText, Option<string> Html)> ValidateBody(string? plainText, string? html) =>
            plainText is { Length: > 0 } || html is { Length: > 0 }
                // Bang required to remove compiler warning. Null is implicitly converted to None.
                ? Success<string, (Option<string> PlainText, Option<string> Html)>((plainText!, html!)) : Fail<string, (Option<string> PlainText, Option<string> Html)>("Body is missing or empty.");

        static Validation<string, string> ValidateSender(Func<string, bool> isValidSender, string email) =>
            isValidSender(email) ? Success<string, string>(email) : Fail<string, string>("From is unknown.");

        static Validation<string, Option<Address>> ValidateAddressOptional(Address? address, string context) =>
            address is not null ? Some(ValidateAddress(address, context)).Sequence() : Success<string, Option<Address>>(None);

        static Validation<string, Address> ValidateAddress(Address? address, string context) =>
            from addressNotNull in IsNotNull(address, context)
            from _ in ValidateEmail(addressNotNull.Email, context)
            select addressNotNull;

        static Validation<string, IEnumerable<Address>> ValidateAddressesOptional(IEnumerable<Address>? addresses, string context) =>
            addresses is not null ? ValidateAddresses(addresses, context) : Success<string, IEnumerable<Address>>(Enumerable.Empty<Address>());

        static Validation<string, IEnumerable<Address>> ValidateAddresses(IEnumerable<Address>? addresses, string context) =>
            from addressesNotNull in IsNotNull(addresses, context)
            from _ in ValidateAddressesAll(addressesNotNull, context)
            select addressesNotNull;

        static Validation<string, IEnumerable<Address>> ValidateAddressesAll(IEnumerable<Address> addresses, string context) =>
            addresses.Map((i, address) => ValidateAddress(address, $"{context} #{i + 1}")).Sequence();

        static Validation<string, string> ValidateEmail(string? email, string context) =>
            from emailNotNull in IsNotNullOrEmpty(email, $"{context} email")
            from _ in IsValidEmail(emailNotNull, $"{context} email")
            select emailNotNull;

        static Validation<string, string> IsValidEmail(string email, string context) =>
            email.Contains('@', StringComparison.Ordinal) ? Success<string, string>(email) : Fail<string, string>($"{context} is not a valid email address.");

        static Validation<string, string> IsNotNullOrEmpty(string? value, string context) =>
            value is { Length: > 0 } ? Success<string, string>(value!) : Fail<string, string>($"{context} is missing or empty.");

        static Validation<string, T> IsNotNull<T>(T? value, string context) where T : class =>
            value is not null ? Success<string, T>(value!) : Fail<string, T>($"{context} is missing.");
    }
}
