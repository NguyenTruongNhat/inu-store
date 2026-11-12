using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Abstractions.Repositories;

/// <summary>Provides permission persistence and database-backed rule checks.</summary>
public interface IPermissionRepository : IRepositoryBase<Permission, int>
{
    /// <summary>Returns untracked permissions ordered by identifier.</summary>
    Task<List<Permission>> GetAllOrderedByIdAsync(CancellationToken cancellationToken = default);
    /// <summary>Checks name uniqueness using the database collation, optionally excluding an updated permission.</summary>
    Task<bool> PermissionNameExistsAsync(string permissionName, int? excludedPermissionId, CancellationToken cancellationToken = default);
    /// <summary>Checks whether the permission is assigned to any role.</summary>
    Task<bool> IsAssignedToAnyRoleAsync(int permissionId, CancellationToken cancellationToken = default);
}
