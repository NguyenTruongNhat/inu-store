using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.ApplicationParameter;

namespace INuStore.UseCase.Abstractions.Services;

public interface IApplicationParameterService
{
    Task<Result<Response.GetByKeyResponse>> GetByKeyAsync(
        Query.GetByKey request, CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(Command.Create request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Command.Update request, CancellationToken cancellationToken = default);
}
