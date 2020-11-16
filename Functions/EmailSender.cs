using Frederikskaj2.CloudFunctions.EmailSender;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Frederikskaj2.CloudFunctions.Functions
{
    public class EmailSender
    {
        readonly IEmailService emailService;
        readonly ILogger logger;
        readonly EmailSenderOptions options;
        readonly ISendRequestValidator validator;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSenderOptions> options, ISendRequestValidator validator, IEmailService emailService)
        {
            this.logger = logger;
            this.validator = validator;
            this.emailService = emailService;

            this.options = options.Value;
        }

        [FunctionName(nameof(EmailSender))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request)
        {
            logger.LogInformation("Received request");
            using var streamReader = new StreamReader(request.Body);
            var requestBody = await streamReader.ReadToEndAsync();
            var sendRequest = JsonConvert.DeserializeObject<SendRequest>(requestBody);
            var validation = validator.Validate(sendRequest);
            var result = await validation.MatchAsync(
                SuccAsync: async command =>
                {
                    await emailService.SendAsync(command);
                    return (IActionResult) new NoContentResult();
                },
                Fail: errors => new BadRequestObjectResult(errors));
            return result;
        }
    }
}
