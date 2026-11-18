using INuStore.API.Authorization;
using INuStore.API.Concurrency;
using INuStore.API.Filters;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.User;
using INuStore.Domain.Constants;
using INuStore.UseCase.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers.V2;

[ApiVersion("2")]
public sealed class UserController(IUserService userService) : ApiController
{
    [HasPermission(Permissions.User.CreateUser)]
    [SerializeUserWrites]
    [HttpPost("CreateUser")]
    [Validate<Command.CreateUser>]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser([FromBody] Command.CreateUser request, CancellationToken cancellationToken = default)
    {
        var result = await userService.CreateUserV2(request, cancellationToken);
        return Ok(result);
    }
}
