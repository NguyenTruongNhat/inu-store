using INuStore.Domain.Entities;
using INuStore.UseCase.Abstractions.Repositories;

namespace INuStore.Infrastructure.Repositories;

public sealed class NotificationRepository(ApplicationDbContext context)
    : INotificationRepository
{
    public void Add(Notification notification)
        => context.Notifications.Add(notification);
}
