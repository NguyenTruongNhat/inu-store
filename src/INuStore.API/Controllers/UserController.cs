using INustore.UseCase.Abstractions.Services;
using INuStore.API.Filters;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace INuStore.API.Controllers
{
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById([FromQuery] Query.GetUserById request, CancellationToken cancellationToken = default)
        {
            var response = await _userService.GetUserById(request, cancellationToken);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] Query.GetAllUsers request, CancellationToken cancellationToken = default)
        {
            var response = await _userService.GetAllUsers(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("CreateUser")]
        [Validate<Command.CreateUser>]
        public async Task<IActionResult> CreateUser([FromBody] Command.CreateUser request, CancellationToken cancellationToken = default)
        {
            var result = await _userService.CreateUser(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut("UpdateUser")]
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
        [ProducesResponseType(typeof(Result<Response.GetUserRolesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserRoles([FromRoute, Range(1, int.MaxValue)] int userId, CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetUserRoles(userId, cancellationToken);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{userId:int}/roles")]
        [Validate<Request.UserRolesRoute>]
        [Validate<Command.ReplaceUserRoles>]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReplaceUserRoles(
            [FromRoute] Request.UserRolesRoute route,
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRoleFromUser(
            [FromRoute] Command.RemoveRoleFromUser request,
            CancellationToken cancellationToken = default)
        {
            var result = await _userService.RemoveRoleFromUser(request, cancellationToken);
            return Ok(result);
        }
    }
}
