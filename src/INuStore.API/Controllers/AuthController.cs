using INustore.UseCase.Abstractions.Services;
using INuStore.Contract.Service.V1.Auth;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers
{
    [ApiVersion("1")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Query.Login request, CancellationToken cancellationToken = default)
        {
            return Ok(await _authService.LoginAsync(request, cancellationToken));
        }
    }
}
