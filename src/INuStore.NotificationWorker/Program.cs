using INuStore.NotificationWorker.DependencyInjection.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddNotificationWorker(builder.Configuration);

using var host = builder.Build();

host.UseNotificationRecurringJobs();

await host.RunAsync();
