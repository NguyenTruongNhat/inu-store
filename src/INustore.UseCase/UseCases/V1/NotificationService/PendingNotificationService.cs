using INuStore.Domain.Enums;
using INuStore.UseCase.Abstractions.Notifications;
using INuStore.UseCase.Abstractions.UnitOfWork;
using INuStore.UseCase.Exceptions;

namespace INuStore.UseCase.UseCases.V1.NotificationService;

public sealed class PendingNotificationService(IUnitOfWork unitOfWork, IEmailSender emailSender)
{
    private const int MaxAttempts = 5;

    public async Task SendDueEmailsAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var notifications = await unitOfWork.NotificationRepository.GetDueAsync(
            NotificationChannel.Email,
            DateTime.UtcNow,
            batchSize,
            cancellationToken);

        foreach (var notification in notifications)
        {
            try
            {
                var providerMessageId = await emailSender.SendEmailAsync(
                    notification.Recipient,
                    notification.Subject ?? throw new InvalidOperationException("Email notification requires a subject."),
                    notification.Body,
                    notification.Id.ToString("N"),
                    cancellationToken);

                notification.AttemptCount++;
                notification.Status = NotificationStatus.Sent;
                notification.SentAtUtc = DateTime.UtcNow;
                notification.ProviderMessageId = providerMessageId;
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (EmailProviderConfigurationException)
            {
                // This exception keep notification in pending state, so it will be retried later.
                throw;
            }
            catch (Exception)
            {
                notification.AttemptCount++;

                notification.Status = notification.AttemptCount >= MaxAttempts
                    ? NotificationStatus.Failed
                    : NotificationStatus.Pending;

                notification.NextAttemptAtUtc =
                    DateTime.UtcNow.AddMinutes(5);
            }

            // Worker not use Middleware to handle transaction,
            // so we need to save changes after each notification processing.
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
