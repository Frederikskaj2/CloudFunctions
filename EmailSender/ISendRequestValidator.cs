using LanguageExt;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public interface ISendRequestValidator
    {
        Validation<string, SendCommand> Validate(SendRequest request);
    }
}
