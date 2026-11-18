using INuStore.API.Authorization;
using INuStore.API.Filters;
using INuStore.API.Concurrency;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.User;
using INuStore.Domain.Enums;
using INuStore.Domain.Constants;
using INuStore.UseCase.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

[ApiVersion("1")]
public class UserController : ApiController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [Authorize(Policy = "allow-gmail")]
    [HttpGet("GetUserById")]
    [Validate<Query.GetUserById>]
    [ProducesResponseType(typeof(Result<Response.GetUserByIdResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById([FromQuery] Query.GetUserById request, CancellationToken cancellationToken = default)
    {
        var response = await _userService.GetUserById(request, cancellationToken);
        return Ok(response);
    }

    [HasPermission("User::GetAllUsers")]
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers([FromQuery] Query.GetAllUsers request, CancellationToken cancellationToken = default)
    {
        var response = await _userService.GetAllUsers(request, cancellationToken);
        return Ok(response);
    }

    [HasPermission(Permissions.User.CreateUser)]
    [SerializeUserWrites]
    [HttpPost("CreateUser")]
    [Validate<Command.CreateUser>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser([FromBody] Command.CreateUser request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.CreateUser(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("UpdateUser")]
    [SerializeUserWrites]
    [Validate<Command.UpdateUser>]
    public async Task<IActionResult> UpdateUser([FromBody] Command.UpdateUser request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateUser(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromBody] Command.DeleteUser request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.DeleteUser(request, cancellationToken);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("{userId:int}/roles")]
    [Validate<Request.UserIdRoute>]
    [ProducesResponseType(typeof(Result<Response.GetUserRolesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoles([FromRoute] Request.UserIdRoute route, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetUserRoles(route.UserId, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = nameof(RoleType.Admin))]
    [HttpGet("{userId:int}/permissions")]
    [Validate<Request.UserIdRoute>]
    [ProducesResponseType(typeof(Result<Response.GetUserPermissionsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPermissions(
        [FromRoute] Request.UserIdRoute route,
        CancellationToken cancellationToken)
        => Ok(await _userService.GetUserPermissions(route.UserId, cancellationToken));

    [Authorize]
    [HttpPut("{userId:int}/roles")]
    [Validate<Request.UserIdRoute>]
    [Validate<Command.ReplaceUserRoles>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReplaceUserRoles(
        [FromRoute] Request.UserIdRoute route,
        [FromBody] Command.ReplaceUserRoles request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.ReplaceUserRoles(route.UserId, request, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{userId:int}/roles/{roleId:int}")]
    [Validate<Command.AssignRoleToUser>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignRoleToUser(
        [FromRoute] Command.AssignRoleToUser request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.AssignRoleToUser(request, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{userId:int}/roles/{roleId:int}")]
    [Validate<Command.RemoveRoleFromUser>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveRoleFromUser(
        [FromRoute] Command.RemoveRoleFromUser request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.RemoveRoleFromUser(request, cancellationToken);
        return Ok(result);
    }
}
