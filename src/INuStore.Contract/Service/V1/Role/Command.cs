namespace INuStore.Contract.Service.V1.Role;

public static class Command
{
    public sealed record CreateRole(string Name, string? Description);

    public sealed record UpdateRole(int RoleId, string Name, string? Description);

    public sealed record DeleteRole(int RoleId);
}
