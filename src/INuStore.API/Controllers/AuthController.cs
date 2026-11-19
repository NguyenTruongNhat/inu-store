using INuStore.API.Filters;
using INuStore.Contract.Service.V1.Auth;
using INuStore.UseCase.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

[ApiVersion("1")]
[AllowAnonymous]
public class AuthController : ApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("Register")]
    [Validate<Command.Register>]
    public async Task<IActionResult> Register([FromBody] Command.Register request, CancellationToken cancellationToken = default)
        => Ok(await _authService.RegisterAsync(request, cancellationToken));

    [HttpPost("Login")]
    [Validate<Query.Login>]
    public async Task<IActionResult> Login([FromBody] Query.Login request, CancellationToken cancellationToken = default)
    {
        return Ok(await _authService.LoginAsync(request, cancellationToken));
    }

    [HttpPost("RefreshToken")]
    [Validate<Query.Token>]
    public async Task<IActionResult> RefreshToken([FromBody] Query.Token request, CancellationToken cancellationToken = default)
        => Ok(await _authService.RefreshTokenAsync(request, cancellationToken));

    [HttpPost("RevokeToken")]
    [Validate<Command.Revoke>]
    public async Task<IActionResult> RevokeToken([FromBody] Command.Revoke request, CancellationToken cancellationToken = default)
        => Ok(await _authService.RevokeTokenAsync(request, cancellationToken));
}
