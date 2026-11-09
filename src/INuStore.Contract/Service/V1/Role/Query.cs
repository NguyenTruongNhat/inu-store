namespace INuStore.Contract.Service.V1.Role;

public static class Query
{
    /// <summary>Retrieves a role by its identifier.</summary>
    public sealed record GetRoleById(int RoleId);
}
