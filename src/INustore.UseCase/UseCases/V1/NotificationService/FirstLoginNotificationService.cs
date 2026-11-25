using INuStore.Domain.Entities;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Enums;
using INuStore.UseCase.Abstractions.Notifications;
using INuStore.UseCase.Abstractions.UnitOfWork;
using INuStore.UseCase.EmailTemplates;

namespace INuStore.UseCase.UseCases.V1.NotificationService;

public sealed class FirstLoginNotificationService(IUnitOfWork unitOfWork, IEmailTemplateRenderer emailTemplateRenderer)
{
    public async Task ScheduleAsync(User user, CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;

        var firstLogin = await unitOfWork.UserRepository.TryMarkFirstLoginAsync(user.Id, nowUtc, cancellationToken);

        if (!firstLogin)
        {
            return;
        }

        var bodyEmail = emailTemplateRenderer.Render("Welcome",
                                    new Dictionary<string, string>
                                    {
                                        ["UserName"] = user.UserName
                                    });

        unitOfWork.NotificationRepository.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            EventType = NotificationType.FirstLogin,
            Channel = NotificationChannel.Email,
            Recipient = user.Email,
            Subject = "Welcome to INuStore",
            Body = bodyEmail,
            Status = NotificationStatus.Pending,
            CreatedAtUtc = nowUtc,
            NextAttemptAtUtc = nowUtc
        });
    }
}
