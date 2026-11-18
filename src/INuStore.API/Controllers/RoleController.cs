using INuStore.API.Filters;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Role;
using INuStore.Domain.Enums;
using INuStore.UseCase.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

[Authorize]
[ApiVersion("1")]
public class RoleController(IRoleService roleService) : ApiController
{
    [HttpGet("GetAllRoles")]
    [Authorize(Roles = nameof(RoleType.Admin))]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken = default)
        => Ok(await roleService.GetAllRoles(cancellationToken));

    [HttpGet("GetRoleById")]
    [Authorize(Roles = nameof(RoleType.Manager))]
    [Validate<Query.GetRoleById>]
    [ProducesResponseType(typeof(Result<Response.GetRoleByIdResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoleById([FromQuery] Query.GetRoleById request, CancellationToken cancellationToken = default)
        => Ok(await roleService.GetRoleById(request, cancellationToken));

    [HttpPost("CreateRole")]
    [Validate<Command.CreateRole>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRole([FromBody] Command.CreateRole request, CancellationToken cancellationToken = default)
        => Ok(await roleService.CreateRole(request, cancellationToken));

    [HttpPut("UpdateRole")]
    [Validate<Command.UpdateRole>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRole([FromBody] Command.UpdateRole request, CancellationToken cancellationToken = default)
        => Ok(await roleService.UpdateRole(request, cancellationToken));

    [HttpPatch("{roleId:int}/permissions")]
    [Authorize(Roles = nameof(RoleType.Admin))]
    [Validate<Request.RoleIdRoute>]
    [Validate<Command.AssignPermissions>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignPermissions([FromRoute] Request.RoleIdRoute route, [FromBody] Command.AssignPermissions request,
        CancellationToken cancellationToken)
        => Ok(await roleService.AssignPermissions(route.RoleId, request, cancellationToken));

    [HttpDelete("{roleId:int}/permissions/{permissionId:int}")]
    [Authorize(Roles = nameof(RoleType.Admin))]
    [Validate<Request.RolePermissionRoute>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokePermission(
        [FromRoute] Request.RolePermissionRoute route,
        CancellationToken cancellationToken)
        => Ok(await roleService.RevokePermission(route.RoleId, route.PermissionId, cancellationToken));

    [HttpDelete("DeleteRole")]
    [Validate<Command.DeleteRole>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRole([FromBody] Command.DeleteRole request, CancellationToken cancellationToken = default)
        => Ok(await roleService.DeleteRole(request, cancellationToken));
}
