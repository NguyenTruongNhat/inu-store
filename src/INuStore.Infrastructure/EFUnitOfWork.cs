using INustore.UseCase.Abstractions.Repositories;
using INustore.UseCase.Abstractions.UnitOfWork;
using INuStore.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository UserRepository { get; }
    public IRoleRepository RoleRepository { get; }

    public EFUnitOfWork(ApplicationDbContext context, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _context = context;
        UserRepository = userRepository;
        RoleRepository = roleRepository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            var translatedException = SqlServerExceptionTranslator.Translate(exception);
            if (translatedException is not null)
            {
                throw translatedException;
            }

            throw;
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
        => await _context.DisposeAsync();
}
