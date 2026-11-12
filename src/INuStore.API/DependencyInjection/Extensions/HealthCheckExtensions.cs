using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace INuStore.API.DependencyInjection.Extensions;

public static class HealthCheckExtensions
{
    public static IEndpointRouteBuilder MapAppHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        }).AllowAnonymous();

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready")
        }).AllowAnonymous();

        return endpoints;
    }
}
