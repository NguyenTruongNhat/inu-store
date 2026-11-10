using INustore.UseCase.Abstractions.Repositories;
using INuStore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Repositories;

/// <summary>Persists roles using the request's shared EF Core context.</summary>
public sealed class RoleRepository : RepositoryBase<Role, int>, IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => FindAll().OrderBy(role => role.Id).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<List<Role>> GetByIdsAsync(IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default)
        => FindAll(role => roleIds.Contains(role.Id)).Distinct().ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<bool> NameExistsAsync(string name, int? excludedRoleId = null, CancellationToken cancellationToken = default)
        => FindAll(role => role.Name == name && (!excludedRoleId.HasValue || role.Id != excludedRoleId.Value))
            .AnyAsync(cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasAssignmentsAsync(int roleId, CancellationToken cancellationToken = default)
        => _context.UserRoles.AnyAsync(assignment => assignment.RoleId == roleId, cancellationToken);
}
