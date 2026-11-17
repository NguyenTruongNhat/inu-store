using Microsoft.AspNetCore.Authorization;

namespace INuStore.API.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute, IAuthorizationRequirementData
{
    private readonly PermissionRequirement _requirement;

    public HasPermissionAttribute(params string[] permissionNames)
    {
        ArgumentNullException.ThrowIfNull(permissionNames);
        if (permissionNames.Length == 0 || permissionNames.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Specify at least one permission name. Each name must be nonempty.", nameof(permissionNames));
        }

        _requirement = new PermissionRequirement(permissionNames.Select(name => name.Trim()).ToArray());
    }

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return _requirement;
    }
}
