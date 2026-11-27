using INuStore.Contract.Service.V1.File;
using INuStore.Domain.Entities.Files;
using INuStore.UseCase.Abstractions.Storage;
using INuStore.UseCase.Abstractions.UnitOfWork;

namespace INuStore.UseCase.UseCases.V1.FileService;

public sealed class FileService(IEnumerable<IFileStorage> providers, IUnitOfWork unitOfWork)
{
    public async Task<Response.FileUploadResponse> UploadAsync(Stream content, string originalFileName, string? contentType,
                                                                CancellationToken cancellationToken)
    {
        var parameter = await unitOfWork.ApplicationParameterRepository.GetByKeyAsync("FileStorageProvider", cancellationToken);

        var storage = providers.SingleOrDefault(provider =>
            string.Equals(provider.Provider.ToString(), parameter?.Value, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("FileStorageProvider is missing or unsupported.");

        var id = Guid.NewGuid();
        var key = id.ToString("N");

        var size = await storage.SaveAsync(key, content, cancellationToken);

        try
        {
            var file = new StoredFile
            {
                Id = id,
                OriginalFileName = originalFileName,
                ContentType = string.IsNullOrWhiteSpace(contentType)
                    ? "application/octet-stream"
                    : contentType,
                Size = size,
                StorageProvider = storage.Provider,
                StorageKey = key,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            unitOfWork.StoredFileRepository.Add(file);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new Response.FileUploadResponse(
                file.Id,
                file.OriginalFileName,
                file.ContentType,
                file.Size,
                file.StorageProvider.ToString());
        }
        catch
        {
            await storage.DeleteAsync(key, CancellationToken.None);
            throw;
        }
    }
}
