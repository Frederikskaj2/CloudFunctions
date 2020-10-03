using Frederikskaj2.CloudFunctions.Validation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Frederikskaj2.CloudFunctions.Validation.ValidationRuleFactory;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    [SuppressMessage("Microsoft.Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated by the framework.")]
    class SendRequestValidator : ISendRequestValidator
    {
        readonly ComposableValidationRule<SendRequest, string> validator;

        public SendRequestValidator(IOptions<EmailSenderOptions> options)
        {
            var validFromEmails = options.Value.Senders.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var emailHasValue = Rule<Address, string>(address => string.IsNullOrEmpty(address.Email), "Email is missing or empty.");
            var emailIsValid = Rule<Address, string>(address => !(address.Email?.Contains('@', StringComparison.Ordinal) ?? false), "Email is invalid.");
            var addressValidator = emailHasValue.ToComposable().And(emailIsValid);

            var fromHasValue = Rule<SendRequest, string>(mail => mail.From is null, "From is missing.");
            var fromIsValid = ChildRule<SendRequest, Address, string>(mail => addressValidator(mail.From), "From is invalid.");
            var fromIsKnown = Rule<SendRequest, string>(mail => !validFromEmails.Contains(mail?.From?.Email ?? string.Empty), "From is unknown.");
            var fromValidator = fromHasValue.ToComposable().And(fromIsValid).And(fromIsKnown);

            var replyToValidator = ChildRule<SendRequest, Address, string>(email => !(email.ReplyTo is null), mail => addressValidator(mail.ReplyTo), "ReplyTo is invalid.");

            var toHasValue = Rule<SendRequest, string>(mail => mail.To == null || !mail.To.Any(), "To is missing or empty.");
            var toIsValid = ChildCollectionRule<SendRequest, Address, string>(request => request.To, address => addressValidator(address), "To is invalid.", i => $"To[{i}] is invalid.");
            var toValidator = toHasValue.ToComposable().And(toIsValid);

            var ccValidator = ChildCollectionRule<SendRequest, Address, string>(request => request.Cc, address => addressValidator(address), "Cc is invalid.", i => $"To[{i}] is invalid.");

            var bccValidator = ChildCollectionRule<SendRequest, Address, string>(request => request.Bcc, address => addressValidator(address), "Bcc is invalid.", i => $"To[{i}] is invalid.");

            var bodyValidator = Rule<SendRequest, string>(mail => string.IsNullOrEmpty(mail.PlainTextBody) && string.IsNullOrEmpty(mail.HtmlBody), "Body is missing or empty.");

            validator = fromValidator
                .Or(replyToValidator)
                .Or(toValidator)
                .Or(ccValidator)
                .Or(bccValidator)
                .Or(bodyValidator);
        }

        public IEnumerable<ValidationError<string>> Validate(SendRequest request) => validator(request).Errors;
    }
}
