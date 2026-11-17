using Microsoft.AspNetCore.Authorization;

namespace INuStore.API.Authorization;

internal sealed class PermissionRequirement(IReadOnlyList<string> permissionNames) : IAuthorizationRequirement
{
    public IReadOnlyList<string> PermissionNames { get; } = permissionNames;
}
