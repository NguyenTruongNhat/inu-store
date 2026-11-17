using INuStore.UseCase.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;

namespace INuStore.API.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var hasPermission = context.User.Identities
            .Where(identity => identity.IsAuthenticated)
            .SelectMany(identity => identity.Claims)
            .Any(claim => claim.Type == CustomClaimTypes.Permissions
                && requirement.PermissionNames.Contains(claim.Value, StringComparer.Ordinal));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
