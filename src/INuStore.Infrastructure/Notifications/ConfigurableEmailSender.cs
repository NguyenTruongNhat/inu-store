using INuStore.UseCase.Abstractions.Notifications;
using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace INuStore.Infrastructure.Notifications;

public sealed class ConfigurableEmailSender(IApplicationParameterRepository parameterRepository, IServiceProvider services) : IEmailSender
{
    public async Task<string> SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var parameter = await parameterRepository.GetByKeyAsync("EmailProvider", cancellationToken);

        var provider = parameter?.Value.Trim().ToLowerInvariant();

        try
        {
            IEmailSender sender = provider switch
            {
                "resend" => services.GetRequiredService<ResendEmailSender>(),
                "ses" => services.GetRequiredService<SesEmailSender>(),
                _ => throw new EmailProviderConfigurationException(
                    "EmailProvider must be 'resend' or 'ses'.")
            };

            return await sender.SendEmailAsync(to, subject, htmlBody, idempotencyKey, cancellationToken);
        }
        catch (OptionsValidationException exception)
        {
            throw new EmailProviderConfigurationException($"Configuration for email provider '{provider}' is incomplete.", exception);
        }
    }
}
