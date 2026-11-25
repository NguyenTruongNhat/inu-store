using Hangfire;
using Hangfire.SqlServer;
using INuStore.Infrastructure.DependencyInjection.Extensions;
using INuStore.NotificationWorker.Jobs;
using INuStore.UseCase.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace INuStore.NotificationWorker.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationWorker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ConnectionStrings")
            ?? throw new InvalidOperationException("SQL Server connection string is required.");

        // Register repositories, email delivery, and notification processing services.
        services.AddNotificationWorkerInfrastructure(configuration);
        services.AddNotificationWorkerUseCase();
        services.AddTransient<SendPendingNotificationsJob>();

        // Hangfire stores and retrieves jobs in SQL Server.
        services.AddHangfire(hangfire => hangfire
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                connectionString,
                new SqlServerStorageOptions
                {
                    PrepareSchemaIfNecessary = true
                }));

        // Use one worker and poll every second to support the six-second cron schedule.
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
        });

        return services;
    }
}
