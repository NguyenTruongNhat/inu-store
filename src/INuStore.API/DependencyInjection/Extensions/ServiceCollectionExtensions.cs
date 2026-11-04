using ApiGateway.DependencyInjection.Extensions;
using INuStore.API.Middlewares;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace INuStore.API.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ExceptionHandlingMiddleware>();
            services.AddScoped<TransactionMiddleware>();

            services.AddControllers();
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddSwagger();

            services.AddJwtAuthenticationApi(configuration);

            return services;
        }
    }
}
