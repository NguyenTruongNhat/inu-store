using INuStore.Contract.Service.V1.User;
using INuStore.Contract.Abstractions.Share;

namespace INustore.UseCase.Abstractions.Services
{
    public interface IUserService
    {
        public Task<Result<Response.GetUserByIdResponse>> GetUserById(Query.GetUserById request, CancellationToken cancellationToken = default);
        public Task<Result<Response.GetAllUsersResponse>> GetAllUsers(Query.GetAllUsers request, CancellationToken cancellationToken = default);
        public Task<Result> CreateUser(Command.CreateUser request, CancellationToken cancellationToken = default);
        public Task<Result> UpdateUser(Command.UpdateUser request, CancellationToken cancellationToken = default);
        public Task<Result> DeleteUser(Command.DeleteUser request, CancellationToken cancellationToken = default);
        public Task<Result<Response.GetUserRolesResponse>> GetUserRoles(int userId, CancellationToken cancellationToken = default);
        public Task<Result> AssignRoleToUser(Command.AssignRoleToUser request, CancellationToken cancellationToken = default);
        public Task<Result> RemoveRoleFromUser(Command.RemoveRoleFromUser request, CancellationToken cancellationToken = default);

    }
}
