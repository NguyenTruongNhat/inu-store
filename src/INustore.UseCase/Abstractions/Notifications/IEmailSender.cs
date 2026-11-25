namespace INuStore.UseCase.Abstractions.Notifications;

public interface IEmailSender
{
    Task<string> SendEmailAsync(
     string to,
     string subject,
     string htmlBody,
     string idempotencyKey,
     CancellationToken cancellationToken = default);
}
