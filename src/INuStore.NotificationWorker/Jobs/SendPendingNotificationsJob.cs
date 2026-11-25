using Hangfire;
using INuStore.UseCase.UseCases.V1.NotificationService;

namespace INuStore.NotificationWorker.Jobs;

public sealed class SendPendingNotificationsJob(PendingNotificationService notificationService)
{
    [DisableConcurrentExecution(timeoutInSeconds: 600)]
    [AutomaticRetry(Attempts = 0)]
    public Task ExecuteAsync(CancellationToken cancellationToken)
        => notificationService.SendDueEmailsAsync(batchSize: 20, cancellationToken);
}
