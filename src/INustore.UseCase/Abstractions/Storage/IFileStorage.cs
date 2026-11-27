using INuStore.Domain.Entities.Files;

namespace INuStore.UseCase.Abstractions.Storage;

public interface IFileStorage
{
    FileStorageProvider Provider { get; }

    Task<long> SaveAsync(string key, Stream content, CancellationToken cancellationToken);

    Task DeleteAsync(string key, CancellationToken cancellationToken);
}
