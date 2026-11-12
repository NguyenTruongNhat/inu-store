namespace INuStore.Contract.Service.V1.Permission;

public static class Command
{
    public sealed record CreatePermission(string Name, string? Description);

    public sealed record UpdatePermission(int PermissionId, string Name, string? Description);

    public sealed record DeletePermission(int PermissionId);
}
