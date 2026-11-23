namespace INuStore.UseCase.Abstractions.Notifications;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body);
}
