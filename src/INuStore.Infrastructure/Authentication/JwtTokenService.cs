using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.UseCase.Abstractions.Security;
using INuStore.UseCase.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace INuStore.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOption jwtOption = new JwtOption();

    public JwtTokenService(IConfiguration configuration)
    {
        configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);
    }
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SecretKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
            issuer: jwtOption.Issuer,
            audience: jwtOption.Audience,
            claims: claims.Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())),
            expires: DateTime.UtcNow.AddMinutes(jwtOption.ExpireMin),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var Key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateLifetime = false, //here we are saying that we don't care about the token's expiration date
            ValidateIssuerSigningKey = true,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ClockSkew = TimeSpan.Zero
        };

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidTokenException();
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256
                || jwtSecurityToken.Payload.Expiration is null)
            {
                throw new InvalidTokenException();
            }

            return principal;
        }
        catch (Exception exception) when (exception is SecurityTokenException or ArgumentException or FormatException)
        {
            throw new InvalidTokenException();
        }
    }
}
