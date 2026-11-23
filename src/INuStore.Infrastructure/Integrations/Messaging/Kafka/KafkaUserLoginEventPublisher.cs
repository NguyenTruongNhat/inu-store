using System.Globalization;
using System.Text.Json;
using Confluent.Kafka;
using INuStore.Contract.Messaging.V1;
using INuStore.Infrastructure.DependencyInjection.Options;
using INuStore.UseCase.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INuStore.Infrastructure.Integrations.Messaging.Kafka;

public sealed class KafkaUserLoginEventPublisher(
    IProducer<string, string> producer,
    IOptions<KafkaOptions> options,
    ILogger<KafkaUserLoginEventPublisher> logger) : IUserLoginEventPublisher
{
    private readonly KafkaOptions _options = options.Value;

    public async Task PublishAsync(UserLoggedIn message, CancellationToken cancellationToken = default)
    {
        try
        {
            var delivery = await producer.ProduceAsync(_options.UserLoggedInTopic, new Message<string, string>
            {
                Key = message.UserId.ToString(CultureInfo.InvariantCulture),
                Value = JsonSerializer.Serialize(message)
            }, cancellationToken);
        }
        catch (KafkaException exception)
        {
            // Learning demo: losing a notification must not prevent login.
            logger.LogWarning("Could not publish login event {MessageId} for user {UserId}. Kafka error: {ErrorCode}. Login will continue.",
                message.MessageId, message.UserId, exception.Error.Code);
        }
    }
}
