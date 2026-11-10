using INustore.UseCase.Abstractions.Repositories;

namespace INustore.UseCase.Abstractions.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }

    /// <summary>
    /// Call save change from db context
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
