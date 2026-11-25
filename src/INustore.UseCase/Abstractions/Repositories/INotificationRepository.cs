using INuStore.Domain.Entities;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface INotificationRepository
{
    void Add(Notification notification);
}
