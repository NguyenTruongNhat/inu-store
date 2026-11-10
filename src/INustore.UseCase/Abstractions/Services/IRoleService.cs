using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Role;

namespace INuStore.UseCase.Abstractions.Services;

/// <summary>Reads roles and stages role changes for the current unit of work.</summary>
public interface IRoleService
{
    /// <summary>Returns all roles ordered by identifier.</summary>
    Task<Result<Response.GetAllRolesResponse>> GetAllRoles(CancellationToken cancellationToken = default);
    /// <summary>Returns the requested role, or throws when it does not exist.</summary>
    Task<Result<Response.GetRoleByIdResponse>> GetRoleById(Query.GetRoleById request, CancellationToken cancellationToken = default);
    /// <summary>Stages a custom role with a unique name.</summary>
    Task<Result> CreateRole(Command.CreateRole request, CancellationToken cancellationToken = default);
    /// <summary>Stages changes while protecting built-in names.</summary>
    Task<Result> UpdateRole(Command.UpdateRole request, CancellationToken cancellationToken = default);
    /// <summary>Stages deletion of an unassigned custom role.</summary>
    Task<Result> DeleteRole(Command.DeleteRole request, CancellationToken cancellationToken = default);
}
