using INuStore.UseCase.Abstractions.Repositories;
using INuStore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Repositories;

/// <summary>Persists permissions using the request's shared EF Core context.</summary>
public sealed class PermissionRepository : RepositoryBase<Permission, int>, IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes the repository with the shared context.</summary>
    /// <param name="context">The context for the current request.</param>
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<List<Permission>> GetAllOrderedByIdAsync(CancellationToken cancellationToken = default)
        => FindAll()
            .OrderBy(permission => permission.Id)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<bool> PermissionNameExistsAsync(string permissionName, int? excludedPermissionId, CancellationToken cancellationToken = default)
    {
        var permissionsWithMatchingName = FindAll(permission => permission.Name == permissionName);
        if (excludedPermissionId is int permissionId)
        {
            permissionsWithMatchingName = permissionsWithMatchingName.Where(permission => permission.Id != permissionId);
        }

        return permissionsWithMatchingName.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> IsAssignedToAnyRoleAsync(int permissionId, CancellationToken cancellationToken = default)
        => _context.Set<RolePermission>()
            .AnyAsync(rolePermission => rolePermission.PermissionId == permissionId, cancellationToken);
}
