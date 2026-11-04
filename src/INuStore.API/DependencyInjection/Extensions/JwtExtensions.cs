using INuStore.Infrastructure.DependencyInjection.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiGateway.DependencyInjection.Extensions;

public static class JwtExtensions
{
    public static void AddJwtAuthenticationApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            JwtOption jwtOption = new JwtOption();
            configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);

            /**
             * Storing the JWT in the AuthenticationProperties allows you to retrieve it from elsewhere within your application.
             * public async Task<IActionResult> SomeAction()
                {
                    // using Microsoft.AspNetCore.Authentication;
                    var accessToken = await HttpContext.GetTokenAsync("access_token");
                    // ...
                }
             */
            o.SaveToken = true; // Save token into AuthenticationProperties

            var Key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false, // on production make it true
                ValidateAudience = false, // on production make it true
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOption.Issuer,
                ValidAudience = jwtOption.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                    }
                    return Task.CompletedTask;
                }
            };

            //o.EventsType = typeof(CustomJwtBearerEvents);
        });

    }


    public static void AddAuthorizationApi(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("allow-gmail", policy =>
            {
                policy.RequireAuthenticatedUser()
                      //.RequireRole("Admin", "User")
                      //.RequireClaim("email", "gmail.com")
                      .RequireAssertion(context =>
                      {
                          var emailClaim = context.User.FindFirst(c => c.Type == "email");
                          return emailClaim != null && emailClaim.Value.EndsWith("@gmail.com");
                      })
                ;
            });
        });
    }



}