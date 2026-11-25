using INuStore.Domain.Entities;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Enums;
using INuStore.UseCase.Abstractions.UnitOfWork;

namespace INuStore.UseCase.UseCases.V1.NotificationService;

public sealed class FirstLoginNotificationService(IUnitOfWork unitOfWork)
{
    private static readonly string WelcomeHtml = LoadWelcomeHtml();
    public async Task ScheduleAsync(User user, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;

        var firstLogin = await unitOfWork.UserRepository.TryMarkFirstLoginAsync(user.Id, nowUtc, cancellationToken);

        if (!firstLogin)
        {
            return;
        }

        unitOfWork.NotificationRepository.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            EventType = NotificationType.FirstLogin,
            Channel = NotificationChannel.Email,
            Recipient = user.Email,
            Subject = "Welcome to INuStore",
            Body = WelcomeHtml,
            Status = NotificationStatus.Pending,
            CreatedAtUtc = nowUtc,
            NextAttemptAtUtc = nowUtc
        });
    }


    private static string LoadWelcomeHtml()
    {
        const string resourceName = "INuStore.UseCase.EmailTemplates.Welcome.html";

        using var stream = typeof(FirstLoginNotificationService)
            .Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Email template resource '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
