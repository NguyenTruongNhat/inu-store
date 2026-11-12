namespace INuStore.Contract.Service.V1.Permission;

public static class Response
{
    public sealed record PermissionResponse(int Id, string Name, string Description);

    public sealed record GetPermissionByIdResponse(PermissionResponse Permission);

    public sealed record GetAllPermissionsResponse(List<PermissionResponse> Permissions);
}
