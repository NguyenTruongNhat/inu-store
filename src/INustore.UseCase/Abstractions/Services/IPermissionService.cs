using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Permission;

namespace INuStore.UseCase.Abstractions.Services;

/// <summary>Reads permissions and stages permission changes for the current unit of work.</summary>
public interface IPermissionService
{
    /// <summary>Returns all permissions ordered by identifier.</summary>
    Task<Result<Response.GetAllPermissionsResponse>> GetAllPermissions(CancellationToken cancellationToken = default);
    /// <summary>Returns the requested permission, or throws when it does not exist.</summary>
    Task<Result<Response.GetPermissionByIdResponse>> GetPermissionById(Query.GetPermissionById request, CancellationToken cancellationToken = default);
    /// <summary>Stages a permission with a unique name.</summary>
    Task<Result> CreatePermission(Command.CreatePermission request, CancellationToken cancellationToken = default);
    /// <summary>Stages changes to a permission.</summary>
    Task<Result> UpdatePermission(Command.UpdatePermission request, CancellationToken cancellationToken = default);
    /// <summary>Stages deletion of an unassigned permission.</summary>
    Task<Result> DeletePermission(Command.DeletePermission request, CancellationToken cancellationToken = default);
}
