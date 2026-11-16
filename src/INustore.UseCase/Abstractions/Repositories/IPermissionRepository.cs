using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, int>
{
    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<int>> GetExistingIdsAsync(IReadOnlyCollection<int> permissionIds, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> PermissionNameExistsAsync(string permissionName, int? excludedPermissionId, CancellationToken cancellationToken = default);
    Task<bool> IsAssignedToAnyRoleAsync(int permissionId, CancellationToken cancellationToken = default);
}
