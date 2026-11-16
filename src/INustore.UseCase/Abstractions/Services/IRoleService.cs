using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Role;

namespace INuStore.UseCase.Abstractions.Services;

public interface IRoleService
{
    Task<Result<Response.GetAllRolesResponse>> GetAllRoles(CancellationToken cancellationToken = default);
    Task<Result<Response.GetRoleByIdResponse>> GetRoleById(Query.GetRoleById request, CancellationToken cancellationToken = default);
    Task<Result> CreateRole(Command.CreateRole request, CancellationToken cancellationToken = default);
    Task<Result> UpdateRole(Command.UpdateRole request, CancellationToken cancellationToken = default);
    Task<Result> DeleteRole(Command.DeleteRole request, CancellationToken cancellationToken = default);
    Task<Result> AssignPermissions(int roleId, Command.AssignPermissions request, CancellationToken cancellationToken = default);
    Task<Result> RevokePermission(int roleId, int permissionId, CancellationToken cancellationToken = default);
}
