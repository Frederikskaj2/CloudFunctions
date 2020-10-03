using Frederikskaj2.CloudFunctions.Validation;
using System.Collections.Generic;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public interface ISendRequestValidator
    {
        IEnumerable<ValidationError<string>> Validate(SendRequest request);
    }
}