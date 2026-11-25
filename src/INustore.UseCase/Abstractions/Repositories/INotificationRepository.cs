using System;
using System.Collections.Generic;
using System.Text;
using INuStore.Domain.Entities;
using INuStore.Domain.Enums;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface INotificationRepository
{
    void Add(Notification notification);

    Task<List<Notification>> GetDueAsync(
        NotificationChannel channel,
        DateTime nowUtc,
        int batchSize,
        CancellationToken cancellationToken = default);
}
