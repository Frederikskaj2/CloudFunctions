using System.Threading.Tasks;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public interface IEmailService
    {
        Task SendAsync(SendCommand command);
    }
}
