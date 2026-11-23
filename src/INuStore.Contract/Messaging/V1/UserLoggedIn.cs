namespace INuStore.Contract.Messaging.V1;

public sealed record UserLoggedIn(Guid MessageId, int UserId, string UserName, string Email, DateTimeOffset OccurredAtUtc);
