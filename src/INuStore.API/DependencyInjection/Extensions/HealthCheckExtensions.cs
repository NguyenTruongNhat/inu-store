using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace INuStore.API.DependencyInjection.Extensions;

/// <summary>Maps the application's operational health endpoints.</summary>
public static class HealthCheckExtensions
{
    /// <summary>Maps anonymous liveness and database readiness probes.</summary>
    /// <param name="endpoints">The application's endpoint route builder.</param>
    /// <returns>The endpoint route builder for further configuration.</returns>
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
