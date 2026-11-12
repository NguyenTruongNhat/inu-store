namespace INuStore.Contract.Service.V1.Permission;

public static class Command
{
    /// <summary>Creates a permission.</summary>
    public sealed record CreatePermission(string Name, string? Description);

    /// <summary>Updates a permission's name and description.</summary>
    public sealed record UpdatePermission(int PermissionId, string Name, string? Description);

    /// <summary>Deletes an unassigned permission.</summary>
    public sealed record DeletePermission(int PermissionId);
}
