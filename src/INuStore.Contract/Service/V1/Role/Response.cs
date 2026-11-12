namespace INuStore.Contract.Service.V1.Role;

public static class Response
{
    public sealed record RoleResponse(int RoleId, string Name, string Description);

    public sealed record GetRoleByIdResponse(RoleResponse Role);

    public sealed record GetAllRolesResponse(List<RoleResponse> Roles);
}
