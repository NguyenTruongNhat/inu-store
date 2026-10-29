using INustore.UseCase.Abstractions.Services;
using INustore.UseCase.UseCases.V1.UserService;
using Microsoft.Extensions.DependencyInjection;

namespace INustore.UseCase.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            return services.AddScoped<IUserService, UserService>();
        }
    }
}
