using System.Security.Claims;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Auth;
using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Enums;
using INuStore.Domain.Exceptions;
using INuStore.UseCase.Abstractions.Caching;
using INuStore.UseCase.Abstractions.Security;
using INuStore.UseCase.Abstractions.Services;
using INuStore.UseCase.Abstractions.UnitOfWork;
using INuStore.UseCase.Exceptions;
using INuStore.UseCase.UseCases.V1.NotificationService;

namespace INuStore.UseCase.UseCases.V1.AuthService;

public class AuthService : IAuthService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICacheService _cacheService;
    private readonly FirstLoginNotificationService _firstLoginNotificationService;

    public AuthService(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ICacheService cacheService, FirstLoginNotificationService firstLoginNotificationService)
    {
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _cacheService = cacheService;
        _firstLoginNotificationService = firstLoginNotificationService;
    }


    public async Task<Result> RegisterAsync(Command.Register request, CancellationToken cancellationToken = default)
    {
        var userName = request.UserName.Trim();
        var email = request.Email.Trim();
        if (await _unitOfWork.UserRepository.ExistsByUserNameAsync(userName, cancellationToken))
        {
            throw new UserException.DuplicateUserNameException();
        }

        var emailAccountCount = await _unitOfWork.UserRepository.CountByEmailForUpdateAsync(
            email, excludedUserId: null, cancellationToken);
        if (emailAccountCount >= 2)
        {
            throw new UserException.EmailAccountLimitExceededException();
        }

        var customerRole = await _unitOfWork.RoleRepository.FindSingleAsync(
            role => role.Name == nameof(RoleType.Customer), cancellationToken)
            ?? throw new InvalidOperationException("The default registration role is not configured.");

        var user = new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAtUtc = DateTime.UtcNow
        };
        user.UserRoles.Add(new UserRole { User = user, RoleId = customerRole.Id });
        _unitOfWork.UserRepository.Add(user);

        return Result.Success();
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
        List<Claim> claims = await BuildClaim(user);

        if (user.UserRoles.Count > 0)
        {
            var permissions = await _unitOfWork.PermissionRepository.GetByUserIdAsync(user.Id, cancellationToken);
            foreach (var permission in permissions)
            {
                claims.Add(new Claim(CustomClaimTypes.Permissions, permission.Name));
            }
        }

        var response = CreateTokens(claims);
        await StoreTokensAsync(user.UserName, response, cancellationToken);

        //send email notification to user
        await _firstLoginNotificationService.ScheduleAsync(user, cancellationToken);
        return Result.Success(response);
    }

    public async Task<Result<Response.Authenticated>> RefreshTokenAsync(Query.Token request, CancellationToken cancellationToken = default)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userName = GetUserName(principal);
        var authenticated = await _cacheService.GetAsync<Response.Authenticated>(userName, cancellationToken);
        if (authenticated is null || string.IsNullOrWhiteSpace(request.RefreshToken)
            || authenticated.RefreshToken != request.RefreshToken
            || authenticated.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new InvalidTokenException();
        }

        // Preserve identity/authorization claims, but let the issuer generate fresh JWT metadata.
        var claims = principal.Claims.Where(claim => claim.Type is not
            ("exp" or "nbf" or "iat" or "jti" or "iss" or "aud"));
        var response = CreateTokens(claims);

        // TODO: Use a Redis transaction or Lua compare-and-set so concurrent refreshes cannot both succeed,
        // and a refresh cannot recreate a session after a concurrent revoke or overwrite a new login.
        await StoreTokensAsync(userName, response, cancellationToken);
        return Result.Success(response);
    }

    public async Task<Result> RevokeTokenAsync(Command.Revoke request, CancellationToken cancellationToken = default)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userName = GetUserName(principal);
        var authenticated = await _cacheService.GetAsync<Response.Authenticated>(userName, cancellationToken) ?? throw new InvalidTokenException();

        // TODO: Make session reads/removal atomic with refresh and login using Redis transactions or Lua.
        await _cacheService.RemoveAsync(userName, cancellationToken);
        return Result.Success();
    }

    private Response.Authenticated CreateTokens(IEnumerable<Claim> claims)
        => new()
        {
            AccessToken = _jwtTokenService.GenerateAccessToken(claims),
            RefreshToken = _jwtTokenService.GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5)
        };

    private Task StoreTokensAsync(string userName, Response.Authenticated response, CancellationToken cancellationToken)
        => _cacheService.SetAsync(userName, response, new DateTimeOffset(response.RefreshTokenExpiryTime), cancellationToken);

    private static string GetUserName(ClaimsPrincipal principal)
    {
        var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new InvalidTokenException();
        }

        return userName;
    }

    private async Task<List<Claim>> BuildClaim(User user)
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
        if (user.UserRoles.Count > 0)
        {
            var permissions = await _unitOfWork.PermissionRepository.GetByUserIdAsync(user.Id);
            foreach (var permission in permissions)
            {
                claims.Add(new Claim(CustomClaimTypes.Permissions, permission.Name));
            }
        }

        return claims;
    }
}
