using INuStore.UseCase.Abstractions.Services;
using INuStore.API.Filters;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Permission;
using INuStore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

/// <summary>Manages the permission catalog.</summary>
[Authorize(Roles = nameof(RoleType.Admin))]
[ApiVersion("1")]
public class PermissionController(IPermissionService permissionService) : ApiController
{
    /// <summary>Returns all permissions ordered by identifier.</summary>
    [HttpGet("GetAllPermissions")]
    [ProducesResponseType(typeof(Result<Response.GetAllPermissionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken = default)
        => Ok(await permissionService.GetAllPermissions(cancellationToken));

    /// <summary>Returns a permission by identifier.</summary>
    [HttpGet("GetPermissionById")]
    [Validate<Query.GetPermissionById>]
    [ProducesResponseType(typeof(Result<Response.GetPermissionByIdResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissionById([FromQuery] Query.GetPermissionById request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.GetPermissionById(request, cancellationToken));

    /// <summary>Creates a permission with a unique name.</summary>
    [HttpPost("CreatePermission")]
    [Validate<Command.CreatePermission>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePermission([FromBody] Command.CreatePermission request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.CreatePermission(request, cancellationToken));

    /// <summary>Updates a permission.</summary>
    [HttpPut("UpdatePermission")]
    [Validate<Command.UpdatePermission>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePermission([FromBody] Command.UpdatePermission request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.UpdatePermission(request, cancellationToken));

    /// <summary>Deletes a permission only when it has no role assignments.</summary>
    [HttpDelete("DeletePermission")]
    [Validate<Command.DeletePermission>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletePermission([FromBody] Command.DeletePermission request, CancellationToken cancellationToken = default)
        => Ok(await permissionService.DeletePermission(request, cancellationToken));
}
