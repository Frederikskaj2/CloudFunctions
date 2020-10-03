using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Frederikskaj2.CloudFunctions.EmailSender
{
    public static class ServiceCollectionExtensions
    {
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "It's the responsibility of the dependency injection container to dispose instances.")]
        public static IServiceCollection AddEmailSender(this IServiceCollection services)
            => services
                .AddSingleton<ISendRequestValidator, SendRequestValidator>()
                .AddSingleton<IEmailService, EmailService>()
                .AddSingleton(new SmtpClient());
    }
}
