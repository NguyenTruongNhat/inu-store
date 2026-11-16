namespace INuStore.Contract.Service.V1.Role;

public static class Request
{
    public sealed record RoleIdRoute(int RoleId);
    public sealed record RolePermissionRoute(int RoleId, int PermissionId);
}
