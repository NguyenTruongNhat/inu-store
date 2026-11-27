using INuStore.Domain.Entities.Files;
using INuStore.UseCase.Abstractions.Repositories;

namespace INuStore.Infrastructure.Repositories;

public sealed class StoredFileRepository(ApplicationDbContext context)
    : IStoredFileRepository
{
    public void Add(StoredFile file) => context.Set<StoredFile>().Add(file);
}
