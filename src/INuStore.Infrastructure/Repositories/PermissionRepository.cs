using INuStore.UseCase.Abstractions.Repositories;
using INuStore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Repositories;

public sealed class PermissionRepository : RepositoryBase<Permission, int>, IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
        => FindAll()
            .OrderBy(permission => permission.Id)
            .ToListAsync(cancellationToken);

    public Task<bool> PermissionNameExistsAsync(string permissionName, int? excludedPermissionId, CancellationToken cancellationToken = default)
    {
        var permissionsWithMatchingName = FindAll(permission => permission.Name == permissionName);
        if (excludedPermissionId is int permissionId)
        {
            permissionsWithMatchingName = permissionsWithMatchingName.Where(permission => permission.Id != permissionId);
        }

        return permissionsWithMatchingName.AnyAsync(cancellationToken);
    }

    public Task<bool> IsAssignedToAnyRoleAsync(int permissionId, CancellationToken cancellationToken = default)
        => _context.Set<RolePermission>()
            .AnyAsync(rolePermission => rolePermission.PermissionId == permissionId, cancellationToken);
}
