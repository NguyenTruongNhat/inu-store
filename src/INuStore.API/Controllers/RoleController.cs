using INustore.UseCase.Abstractions.Services;
using INuStore.API.Filters;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

/// <summary>Manages roles and protects built-in roles and existing assignments.</summary>
[Authorize]
[ApiVersion("1")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RoleController(IRoleService roleService) : ApiController
{
    /// <summary>Returns all roles ordered by identifier.</summary>
    [HttpGet("GetAllRoles")]
    [ProducesResponseType(typeof(Result<Response.GetAllRolesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken = default)
        => Ok(await roleService.GetAllRoles(cancellationToken));

    /// <summary>Returns a role by identifier.</summary>
    [HttpGet("GetRoleById")]
    [Validate<Query.GetRoleById>]
    [ProducesResponseType(typeof(Result<Response.GetRoleByIdResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoleById([FromQuery] Query.GetRoleById request, CancellationToken cancellationToken = default)
        => Ok(await roleService.GetRoleById(request, cancellationToken));

    /// <summary>Creates a custom role with a unique name.</summary>
    [HttpPost("CreateRole")]
    [Validate<Command.CreateRole>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRole([FromBody] Command.CreateRole request, CancellationToken cancellationToken = default)
        => Ok(await roleService.CreateRole(request, cancellationToken));

    /// <summary>Updates a role; built-in names cannot be changed.</summary>
    [HttpPut("UpdateRole")]
    [Validate<Command.UpdateRole>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRole([FromBody] Command.UpdateRole request, CancellationToken cancellationToken = default)
        => Ok(await roleService.UpdateRole(request, cancellationToken));

    /// <summary>Deletes a custom role only when it has no user assignments.</summary>
    [HttpDelete("DeleteRole")]
    [Validate<Command.DeleteRole>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRole([FromBody] Command.DeleteRole request, CancellationToken cancellationToken = default)
        => Ok(await roleService.DeleteRole(request, cancellationToken));
}
