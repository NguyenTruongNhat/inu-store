using INuStore.Domain.Entities.Files;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface IStoredFileRepository
{
    void Add(StoredFile file);
}
