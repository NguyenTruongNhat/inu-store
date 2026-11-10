using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Abstractions.Repositories;

/// <summary>Provides role persistence and database-backed rule checks.</summary>
public interface IRoleRepository : IRepositoryBase<Role, int>
{
    /// <summary>Returns untracked roles ordered by identifier.</summary>
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>Returns untracked roles matching the supplied identifiers in one query.</summary>
    Task<List<Role>> GetByIdsAsync(IReadOnlyCollection<int> roleIds, CancellationToken cancellationToken = default);
    /// <summary>Checks name uniqueness using the database collation, optionally excluding an updated role.</summary>
    Task<bool> NameExistsAsync(string name, int? excludedRoleId = null, CancellationToken cancellationToken = default);
    /// <summary>Checks whether any user is assigned to the role.</summary>
    Task<bool> HasAssignmentsAsync(int roleId, CancellationToken cancellationToken = default);
}
