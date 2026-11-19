using INuStore.API.Filters;
using INuStore.Contract.Service.V1.Permission;
using INuStore.Domain.Enums;
using INuStore.UseCase.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

[Authorize(Roles = nameof(RoleType.Admin))]
[ApiVersion("1")]
public class PermissionController(IPermissionService permissionService) : ApiController
{
    [HttpGet("GetAllPermissions")]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken = default)
        => Ok(await permissionService.GetAllPermissions(cancellationToken));

    [HttpGet("GetPermissionById")]
    [Validate<Query.GetPermissionById>]
    public async Task<IActionResult> GetPermissionById([FromQuery] Query.GetPermissionById request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.GetPermissionById(request, cancellationToken));

    [HttpPost("CreatePermission")]
    [Validate<Command.CreatePermission>]
    public async Task<IActionResult> CreatePermission([FromBody] Command.CreatePermission request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.CreatePermission(request, cancellationToken));

    [HttpPut("UpdatePermission")]
    [Validate<Command.UpdatePermission>]
    public async Task<IActionResult> UpdatePermission([FromBody] Command.UpdatePermission request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.UpdatePermission(request, cancellationToken));

    [HttpDelete("DeletePermission")]
    [Validate<Command.DeletePermission>]
    public async Task<IActionResult> DeletePermission([FromBody] Command.DeletePermission request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.DeletePermission(request, cancellationToken));
}
