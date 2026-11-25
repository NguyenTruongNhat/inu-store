using INuStore.Domain.Entities;
using INuStore.Domain.Enums;
using INuStore.UseCase.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Repositories;

public sealed class NotificationRepository(ApplicationDbContext context)
    : INotificationRepository
{
    public void Add(Notification notification)
        => context.Notifications.Add(notification);

    public Task<List<Notification>> GetDueAsync(
        NotificationChannel channel,
        DateTime nowUtc,
        int batchSize,
        CancellationToken cancellationToken = default)
        => context.Notifications
            .AsTracking()
            .Where(notification =>
                notification.Channel == channel
                && notification.Status == NotificationStatus.Pending
                && notification.NextAttemptAtUtc <= nowUtc)
            .OrderBy(notification => notification.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
}
