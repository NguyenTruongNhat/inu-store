using INuStore.UseCase.Abstractions.Repositories;
using INuStore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Repositories;

public sealed class RoleRepository : RepositoryBase<Role, int>, IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => FindAll().OrderBy(role => role.Id).ToListAsync(cancellationToken);

    public Task<List<Role>> GetByIdsAsync(IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default)
        => FindAll(role => roleIds.Contains(role.Id)).Distinct().ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, int? excludedRoleId = null, CancellationToken cancellationToken = default)
        => FindAll(role => role.Name == name && (!excludedRoleId.HasValue || role.Id != excludedRoleId.Value))
            .AnyAsync(cancellationToken);

    // TODO: Improve concurrency handling when requests change permissions for the same role.
    //.FromSql($"SELECT * FROM [Roles] WITH (UPDLOCK, HOLDLOCK) WHERE [Id] = {roleId}")
    public Task<Role?> GetRoleForPermissionUpdateAsync(int roleId, CancellationToken cancellationToken = default)
        => _context.Roles
            .Include(role => role.RolePermissions)
            .SingleOrDefaultAsync(role => role.Id == roleId, cancellationToken);

    public void AddPermissionAssignment(RolePermission assignment)
        => _context.Set<RolePermission>().Add(assignment);

    public void RemovePermissionAssignment(RolePermission assignment)
        => _context.Set<RolePermission>().Remove(assignment);

    public Task<bool> HasAssignmentsAsync(int roleId, CancellationToken cancellationToken = default)
        => _context.UserRoles.AnyAsync(assignment => assignment.RoleId == roleId, cancellationToken);
}
