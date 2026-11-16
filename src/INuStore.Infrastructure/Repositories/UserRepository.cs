using INuStore.UseCase.Abstractions.Repositories;
using INuStore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Repositories;

public sealed class UserRepository : RepositoryBase<User, int>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        => _context.Users
            .AsTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
            .SingleOrDefaultAsync(user => user.UserName == userName, cancellationToken);

    public Task<User?> GetUserWithRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public Task<bool> ExistsAsync(int userId, CancellationToken cancellationToken = default)
        => _context.Users.AnyAsync(user => user.Id == userId, cancellationToken);

    public void RemoveUserRole(UserRole userRole)
    {
        _context.UserRoles.Remove(userRole);
    }

    public void AddUserRole(UserRole userRole)
    {
        _context.UserRoles.Add(userRole);
    }
}
