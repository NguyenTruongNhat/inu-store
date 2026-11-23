using INuStore.Contract.Messaging.V1;

namespace INuStore.UseCase.Abstractions.Messaging;

public interface IUserLoginEventPublisher
{
    Task PublishAsync(UserLoggedIn message, CancellationToken cancellationToken = default);
}
