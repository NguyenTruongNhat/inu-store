using INustore.UseCase.Abstractions.Security;
using INustore.UseCase.Abstractions.Services;
using INustore.UseCase.Abstractions.UnitOfWork;
using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Auth;
using INuStore.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace INustore.UseCase.UseCases.V1.AuthService
{
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
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(request.UserName, cancellationToken);
            if (user is null)
            {
                throw new Exception("User not found");
            }
            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid password");
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
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                // Add more claims as needed
            };
        }
    }
}
