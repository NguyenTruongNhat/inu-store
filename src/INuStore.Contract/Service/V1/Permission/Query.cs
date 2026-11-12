namespace INuStore.Contract.Service.V1.Permission;

public static class Query
{
    /// <summary>Retrieves a permission by its identifier.</summary>
    public sealed record GetPermissionById(int PermissionId);
}
