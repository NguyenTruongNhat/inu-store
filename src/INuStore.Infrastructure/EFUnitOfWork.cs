using INustore.UseCase.Abstractions.Repositories;
using INustore.UseCase.Abstractions.UnitOfWork;

namespace INuStore.Infrastructure;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository UserRepository { get; }

    public EFUnitOfWork(ApplicationDbContext context, IUserRepository userRepository)
    {
        _context = context;
        UserRepository = userRepository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _context.DisposeAsync();
}
