using INuStore.UseCase.Abstractions.Repositories;

namespace INuStore.UseCase.Abstractions.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }

    IPermissionRepository PermissionRepository { get; }

    INotificationRepository NotificationRepository { get; }
    IApplicationParameterRepository ApplicationParameterRepository { get; }
    IStoredFileRepository StoredFileRepository { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
