namespace INuStore.Contract.Service.V1.User;

public static class Response
{
    public record UserResponse(int UserId, string UserName, string Email);
    public record GetUserByIdResponse(UserResponse User);
    public record GetAllUsersResponse(List<UserResponse> Users);
    public sealed record UserRoleResponse(int RoleId, string Name, string Description);
    public sealed record GetUserRolesResponse(int UserId, List<UserRoleResponse> Roles);
    public sealed record GetUserPermissionsResponse(int UserId, List<Permission.Response.PermissionResponse> Permissions);
}
