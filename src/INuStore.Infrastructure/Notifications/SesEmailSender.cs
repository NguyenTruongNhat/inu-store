using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.UseCase.Abstractions.Notifications;
using Microsoft.Extensions.Options;


namespace INuStore.Infrastructure.Notifications;

public sealed class SesEmailSender(IAmazonSimpleEmailServiceV2 client, IOptions<SesOptions> options) : IEmailSender
{
    public async Task<string> SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var request = new SendEmailRequest
        {
            FromEmailAddress = options.Value.From,
            Destination = new Destination
            {
                ToAddresses = [to]
            },
            Content = new EmailContent
            {
                Simple = new Message
                {
                    Subject = new Content
                    {
                        Data = subject,
                        Charset = "UTF-8"
                    },
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Data = htmlBody,
                            Charset = "UTF-8"
                        }
                    }
                }
            }
        };

        var response = await client.SendEmailAsync(request, cancellationToken);
        return response.MessageId;
    }
}
