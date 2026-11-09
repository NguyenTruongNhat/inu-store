namespace INuStore.Contract.Service.V1.Role;

public static class Command
{
    /// <summary>Creates a role.</summary>
    public sealed record CreateRole(string Name, string? Description);

    /// <summary>Updates a role's name and description.</summary>
    public sealed record UpdateRole(int RoleId, string Name, string? Description);

    /// <summary>Deletes an unassigned custom role.</summary>
    public sealed record DeleteRole(int RoleId);
}
