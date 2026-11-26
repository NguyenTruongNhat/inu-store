using Hangfire;
using INuStore.NotificationWorker.Jobs;

namespace INuStore.NotificationWorker.DependencyInjection.Extensions;

public static class RecurringJobExtensions
{
    public static IHost UseNotificationRecurringJobs(this IHost host)
    {
        var recurringJobs = host.Services.GetRequiredService<IRecurringJobManager>();

        // Create or update one schedule to scan pending notifications every six seconds.
        recurringJobs.AddOrUpdate<SendPendingNotificationsJob>("send-pending-notifications",
                                job => job.ExecuteAsync(CancellationToken.None), "*/6 * * * * *");

        return host;
    }
}
