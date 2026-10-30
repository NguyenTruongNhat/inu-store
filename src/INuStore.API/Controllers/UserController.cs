using INustore.UseCase.Abstractions.Services;
using INuStore.Contract.Service.V1.User;
using INuStore.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUserById")]
        [Validate<Query.GetUserById>]
        public async Task<IActionResult> GetUserById([FromQuery] Query.GetUserById request, CancellationToken cancellationToken = default)
        {
            var response = await _userService.GetUserById(request, cancellationToken);
            return Ok(response);
        }

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
    }
}
