using System.Text.Json;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using INuStore.Contract.Messaging.V1;
using INuStore.Infrastructure.DependencyInjection.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INuStore.Infrastructure.Integrations.Messaging.Kafka;

public sealed class UserLoggedInConsumer(IOptions<KafkaOptions> options, ILogger<UserLoggedInConsumer> logger) : BackgroundService
{
    private readonly KafkaOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await EnsureTopicAsync(stoppingToken);
            using var consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = _options.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                AllowAutoCreateTopics = false
            }).Build();

            consumer.Subscribe(_options.UserLoggedInTopic);
            logger.LogInformation("Listening on Kafka topic {Topic} with group {GroupId}.", _options.UserLoggedInTopic, _options.ConsumerGroupId);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<string, string> result;
                    try
                    {
                        result = consumer.Consume(stoppingToken);
                    }
                    catch (ConsumeException exception) when (!exception.Error.IsFatal)
                    {
                        logger.LogWarning("Kafka consume error {ErrorCode}; retrying in one second.", exception.Error.Code);
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                        continue;
                    }

                    var message = JsonSerializer.Deserialize<UserLoggedIn>(result.Message.Value) ?? throw new JsonException("The login event is null.");
                    if (message.MessageId == Guid.Empty || message.UserId <= 0
                        || string.IsNullOrWhiteSpace(message.UserName) || string.IsNullOrWhiteSpace(message.Email)
                        || message.OccurredAtUtc == default)
                    {
                        throw new JsonException("The login event is missing required fields.");
                    }
                    logger.LogInformation("Received login event {MessageId} for user {UserId} at {TopicPartitionOffset}.",
                        message.MessageId, message.UserId, result.TopicPartitionOffset);
                    consumer.Commit(result);
                }
            }
            finally
            {
                consumer.Close();
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Kafka login consumer stopped.");
        }
        catch (Exception exception)
        {
            // Do not log message payloads or advance past a failed message.
            logger.LogError("Kafka login consumer stopped after {ExceptionType}. Check connectivity, topic permissions, message format and commit status.",
                exception.GetType().Name);
            throw;
        }
    }

    private async Task EnsureTopicAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _options.BootstrapServers
        }).Build();
        var metadata = admin.GetMetadata(TimeSpan.FromSeconds(5));
        logger.LogInformation("Connected to Kafka; discovered {BrokerCount} broker(s).", metadata.Brokers.Count);
        var topic = metadata.Topics.SingleOrDefault(topic => topic.Topic == _options.UserLoggedInTopic);
        if (topic is not null && !topic.Error.IsError)
        {
            return;
        }
        if (topic is not null && topic.Error.Code != ErrorCode.UnknownTopicOrPart)
        {
            throw new KafkaException(topic.Error);
        }

        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await admin.CreateTopicsAsync([new TopicSpecification
            {
                Name = _options.UserLoggedInTopic,
                NumPartitions = 1,
                ReplicationFactor = 1
            }], new CreateTopicsOptions { RequestTimeout = TimeSpan.FromSeconds(5) });
            logger.LogInformation("Created Kafka topic {Topic} for the learning demo.", _options.UserLoggedInTopic);
        }
        catch (CreateTopicsException exception) when (exception.Results.All(result => result.Error.Code == ErrorCode.TopicAlreadyExists))
        {
            // Another worker may have created the topic after our metadata check.
        }
    }
}
