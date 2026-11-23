namespace INuStore.Infrastructure.DependencyInjection.Options;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; init; } = string.Empty;
    public string UserLoggedInTopic { get; init; } = string.Empty;
    public string ConsumerGroupId { get; init; } = string.Empty;
}
