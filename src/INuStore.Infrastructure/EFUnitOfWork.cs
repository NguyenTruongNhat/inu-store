using INuStore.UseCase.Abstractions.Repositories;
using INuStore.UseCase.Abstractions.UnitOfWork;

namespace INuStore.Infrastructure;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }
    public IPermissionRepository PermissionRepository { get; }
    public INotificationRepository NotificationRepository { get; }

    public EFUnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository,
        IPermissionRepository permissionRepository, INotificationRepository notificationRepository)
    {
        _context = context;
        UserRepository = userRepository;
        RoleRepository = roleRepository;
        PermissionRepository = permissionRepository;
        NotificationRepository = notificationRepository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _context.DisposeAsync();
}
