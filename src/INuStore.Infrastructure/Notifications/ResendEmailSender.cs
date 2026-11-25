using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.UseCase.Abstractions.Notifications;
using Microsoft.Extensions.Options;
using Resend;

namespace INuStore.Infrastructure.Notifications;

public sealed class ResendEmailSender(IOptions<ResendOptions> resendOptions) : IEmailSender
{
    private readonly ResendOptions _options = resendOptions.Value;

    public async Task<string> SendEmailAsync(string to, string subject, string htmlBody, string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        IResend resend = ResendClient.Create(_options.ApiKey);

        var response = await resend.EmailSendAsync(idempotencyKey,
            new EmailMessage
            {
                From = _options.From,
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            },
            cancellationToken);

        if (!response.Success)
        {
            throw new InvalidOperationException("Resend rejected the email.");
        }

        return response.Content.ToString();
    }
}
