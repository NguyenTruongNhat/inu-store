using INuStore.UseCase.Abstractions.Security;
using INuStore.UseCase.Abstractions.Services;
using INuStore.UseCase.Abstractions.UnitOfWork;
using INuStore.UseCase.Exceptions;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Auth;
using INuStore.Domain.Entities.Identity;
using System.Security.Claims;

namespace INuStore.UseCase.UseCases.V1.AuthService;

public class AuthService : IAuthService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }


    public async Task<Result<Response.Authenticated>> LoginAsync(Query.Login request, CancellationToken cancellationToken = default)
    {
        // Check User
        var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(request.UserName, cancellationToken) ?? throw new InvalidCredentialsException();
        var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new InvalidCredentialsException();
        }
        List<Claim> claims = BuildClaim(user);

        var accessToken = _jwtTokenService.GenerateAccessToken(claims);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Generate Token
        var response = new Response.Authenticated
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return Result.Success(response);
    }

    private List<Claim> BuildClaim(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
        };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        return claims;
    }
}
