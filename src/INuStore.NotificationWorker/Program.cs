using INuStore.Infrastructure.DependencyInjection.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddKafkaConsumer(builder.Configuration);

await builder.Build().RunAsync();
