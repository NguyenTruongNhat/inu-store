namespace INuStore.Contract.Service.V1.User;

public static class Command
{
    public record CreateUser(string UserName, string Email, string Password);
    public record UpdateUser(int UserId, string UserName, string Email);
    public record DeleteUser(int UserId);

    public sealed record AssignRoleToUser(int UserId, int RoleId);

    public sealed record RemoveRoleFromUser(int UserId, int RoleId);

    public sealed record ReplaceUserRoles(List<int> RoleIds);
}
