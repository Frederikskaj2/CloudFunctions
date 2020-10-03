using Frederikskaj2.CloudFunctions.EmailSender;
using Frederikskaj2.CloudFunctions.Functions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Frederikskaj2.CloudFunctions.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
            => builder.Services
            .AddEmailSender()
            .ConfigureOptions<EmailSenderOptions>("EmailSender");
    }
}
