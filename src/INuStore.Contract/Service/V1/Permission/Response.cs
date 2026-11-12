namespace INuStore.Contract.Service.V1.Permission;

public static class Response
{
    /// <summary>Represents a permission without its role assignments.</summary>
    public sealed record PermissionResponse(int Id, string Name, string Description);

    /// <summary>Contains the requested permission.</summary>
    public sealed record GetPermissionByIdResponse(PermissionResponse Permission);

    /// <summary>Contains all permissions ordered by identifier.</summary>
    public sealed record GetAllPermissionsResponse(List<PermissionResponse> Permissions);
}
