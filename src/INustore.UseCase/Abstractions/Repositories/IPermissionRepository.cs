using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, int>
{
    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> PermissionNameExistsAsync(string permissionName, int? excludedPermissionId, CancellationToken cancellationToken = default);
    Task<bool> IsAssignedToAnyRoleAsync(int permissionId, CancellationToken cancellationToken = default);
}
