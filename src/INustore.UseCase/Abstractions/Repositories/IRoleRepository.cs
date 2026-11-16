using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface IRoleRepository : IRepositoryBase<Role, int>
{
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetByIdsAsync(IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, int? excludedRoleId = null, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleForPermissionUpdateAsync(int roleId, CancellationToken cancellationToken = default);
    void AddPermissionAssignment(RolePermission assignment);
    void RemovePermissionAssignment(RolePermission assignment);
    Task<bool> HasAssignmentsAsync(int roleId, CancellationToken cancellationToken = default);
}
