using INuStore.Domain.Entities;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface IApplicationParameterRepository
{
    Task<ApplicationParameter?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    void Add(ApplicationParameter parameter);
    void Update(ApplicationParameter parameter);
}
