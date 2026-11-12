using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Permission;

namespace INuStore.UseCase.Abstractions.Services;

public interface IPermissionService
{
    Task<Result<Response.GetAllPermissionsResponse>> GetAllPermissions(CancellationToken cancellationToken = default);
    Task<Result<Response.GetPermissionByIdResponse>> GetPermissionById(Query.GetPermissionById request, CancellationToken cancellationToken = default);
    Task<Result> CreatePermission(Command.CreatePermission request, CancellationToken cancellationToken = default);
    Task<Result> UpdatePermission(Command.UpdatePermission request, CancellationToken cancellationToken = default);
    Task<Result> DeletePermission(Command.DeletePermission request, CancellationToken cancellationToken = default);
}
