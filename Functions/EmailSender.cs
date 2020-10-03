using Frederikskaj2.CloudFunctions.EmailSender;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
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
            var errors = validator.Validate(sendRequest).ToList();
            if (errors.Count > 0)
                return new BadRequestObjectResult(string.Join(Environment.NewLine, errors.Select(error => error.ToString(0))));
            await emailService.SendAsync(sendRequest);
            return new NoContentResult();
        }
    }
}
