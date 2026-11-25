using INuStore.Domain.Enums;

namespace INuStore.Domain.Entities;

public class Notification : DomainEntity<Guid>
{
    public int UserId { get; set; }
    public NotificationType EventType { get; set; }
    public NotificationChannel Channel { get; set; }

    public string Recipient { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;

    public NotificationStatus Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTime NextAttemptAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public string? ProviderMessageId { get; set; }
}
