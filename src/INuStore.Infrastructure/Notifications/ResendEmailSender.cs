using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.UseCase.Abstractions.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace INuStore.Infrastructure.Notifications;

public class ResendEmailSender(IOptions<ResendOptions> resendOptions, ILogger<ResendEmailSender> logger) : IEmailSender
{
    private readonly ResendOptions _options = resendOptions.Value;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        IResend resend = ResendClient.Create(_options.ApiKey);

        var response = await resend.EmailSendAsync(new EmailMessage
        {
            From = _options.From,
            To = to,
            Subject = subject,
            HtmlBody = body
        });
        logger.LogInformation("Email accepted by Resend. Email Id={EmailId}", response.Content);
    }
}
