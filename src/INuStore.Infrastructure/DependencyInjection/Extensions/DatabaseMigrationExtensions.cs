using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace INuStore.Infrastructure.DependencyInjection.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DatabaseMigrationExtensions));

        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();

        if (pendingMigrations.Length == 0)
        {
            logger.LogInformation("No pending database migrations. The database is up to date.");
        }
        else
        {
            logger.LogInformation(
                "Applying {MigrationCount} pending database migrations: {MigrationNames}",
                       pendingMigrations.Length, string.Join(", ", pendingMigrations));
        }

        await dbContext.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Database migration step completed successfully.");
    }
}
