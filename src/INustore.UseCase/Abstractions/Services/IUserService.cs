using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.User;

namespace INuStore.UseCase.Abstractions.Services;

public interface IUserService
{
    Task<Result<Response.GetUserByIdResponse>> GetUserById(Query.GetUserById request, CancellationToken cancellationToken = default);
    Task<Result<Response.GetAllUsersResponse>> GetAllUsers(Query.GetAllUsers request, CancellationToken cancellationToken = default);
    Task<Result> CreateUser(Command.CreateUser request, CancellationToken cancellationToken = default);
    Task<Result> UpdateUser(Command.UpdateUser request, CancellationToken cancellationToken = default);
    Task<Result> DeleteUser(Command.DeleteUser request, CancellationToken cancellationToken = default);
    Task<Result<Response.GetUserRolesResponse>> GetUserRoles(int userId, CancellationToken cancellationToken = default);
    Task<Result<Response.GetUserPermissionsResponse>> GetUserPermissions(int userId, CancellationToken cancellationToken = default);
    Task<Result> AssignRoleToUser(Command.AssignRoleToUser request, CancellationToken cancellationToken = default);
    Task<Result> RemoveRoleFromUser(Command.RemoveRoleFromUser request, CancellationToken cancellationToken = default);
    Task<Result> ReplaceUserRoles(int userId, Command.ReplaceUserRoles request, CancellationToken cancellationToken = default);

}
