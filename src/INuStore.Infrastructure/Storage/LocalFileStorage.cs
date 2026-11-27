using INuStore.Domain.Entities.Files;
using INuStore.UseCase.Abstractions.Storage;

namespace INuStore.Infrastructure.Storage;

public sealed class LocalFileStorage(string rootPath) : IFileStorage
{
    public FileStorageProvider Provider => FileStorageProvider.Local;

    public async Task<long> SaveAsync(string key, Stream content, CancellationToken cancellationToken)
    {
        var finalPath = GetPath(key);
        Directory.CreateDirectory(rootPath);

        var tempPath = Path.Combine(rootPath, $"{Guid.NewGuid():N}.tmp");

        var destination = new FileStream(
            tempPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 64 * 1024, // 64 KB buffer
            useAsync: true);

        try
        {
            long size;
            await using (destination)
            {
                await content.CopyToAsync(destination, cancellationToken);
                size = destination.Length;
            }

            cancellationToken.ThrowIfCancellationRequested();
            File.Move(tempPath, finalPath);

            return size;
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        File.Delete(GetPath(key));
        return Task.CompletedTask;
    }

    private string GetPath(string key)
    {
        if (!Guid.TryParseExact(key, "N", out _))
        {
            throw new ArgumentException("Invalid local storage key.", nameof(key));
        }

        return Path.Combine(rootPath, key);
    }
}
