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
    public IApplicationParameterRepository ApplicationParameterRepository { get; }
    public IStoredFileRepository StoredFileRepository { get; }

    public EFUnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository,
        IPermissionRepository permissionRepository, INotificationRepository notificationRepository,
        IApplicationParameterRepository applicationParameterRepository, IStoredFileRepository storedFileRepository)
    {
        _context = context;
        UserRepository = userRepository;
        RoleRepository = roleRepository;
        PermissionRepository = permissionRepository;
        NotificationRepository = notificationRepository;
        ApplicationParameterRepository = applicationParameterRepository;
        StoredFileRepository = storedFileRepository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _context.DisposeAsync();
}
