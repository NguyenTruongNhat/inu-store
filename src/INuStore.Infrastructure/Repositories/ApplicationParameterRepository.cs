using INuStore.Domain.Entities;
using INuStore.UseCase.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Repositories;

public sealed class ApplicationParameterRepository(ApplicationDbContext context) : IApplicationParameterRepository
{
    public Task<ApplicationParameter?> GetByKeyAsync(
        string key, CancellationToken cancellationToken = default)
        => context.ApplicationParameters
            .AsNoTracking()
            .SingleOrDefaultAsync(parameter => parameter.Key == key, cancellationToken);

    public void Add(ApplicationParameter parameter)
        => context.ApplicationParameters.Add(parameter);

    public void Update(ApplicationParameter parameter)
        => context.ApplicationParameters.Update(parameter);
}
