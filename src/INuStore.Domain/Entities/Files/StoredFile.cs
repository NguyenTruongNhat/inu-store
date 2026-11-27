namespace INuStore.Domain.Entities.Files;

public sealed class StoredFile
{
    public Guid Id { get; init; }
    public required string OriginalFileName { get; init; }
    public required string ContentType { get; init; }
    public long Size { get; init; }
    public FileStorageProvider StorageProvider { get; init; }
    public required string StorageKey { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
}
