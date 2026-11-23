using Confluent.Kafka;
using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.Infrastructure.Integrations.Messaging.Kafka;
using INuStore.UseCase.Abstractions.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace INuStore.Infrastructure.DependencyInjection.Extensions;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureKafkaOptions(configuration);
        services.AddSingleton<IProducer<string, string>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<KafkaOptions>>().Value;
            return new ProducerBuilder<string, string>(new ProducerConfig
            {
                BootstrapServers = options.BootstrapServers,
                MessageTimeoutMs = 3000,
                EnableIdempotence = true,
                AllowAutoCreateTopics = false
            }).Build();
        });
        services.AddSingleton<IUserLoginEventPublisher, KafkaUserLoginEventPublisher>();
        return services;
    }

    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureKafkaOptions(configuration)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConsumerGroupId), "Kafka consumer group is required.");
        services.AddHostedService<UserLoggedInConsumer>();
        return services;
    }

    private static OptionsBuilder<KafkaOptions> ConfigureKafkaOptions(this IServiceCollection services, IConfiguration configuration)
        => services.AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection(KafkaOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.BootstrapServers), "Kafka bootstrap servers are required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.UserLoggedInTopic), "Kafka login topic is required.")
            .ValidateOnStart();
}
