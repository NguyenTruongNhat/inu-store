namespace INuStore.Contract.Service.V1.File;

public static class Response
{
    public sealed record FileUploadResponse(
        Guid Id,
        string OriginalFileName,
        string ContentType,
        long Size,
        string StorageProvider);
}
