namespace INuStore.Contract.Service.V1.Role;

public static class Response
{
    /// <summary>Represents a role without its user assignments.</summary>
    public sealed record RoleResponse(int RoleId, string Name, string Description);

    /// <summary>Contains the requested role.</summary>
    public sealed record GetRoleByIdResponse(RoleResponse Role);

    /// <summary>Contains all roles ordered by identifier.</summary>
    public sealed record GetAllRolesResponse(List<RoleResponse> Roles);
}
