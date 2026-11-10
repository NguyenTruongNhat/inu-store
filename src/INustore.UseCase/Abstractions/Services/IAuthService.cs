using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Auth;

namespace INuStore.UseCase.Abstractions.Services;

public interface IAuthService
{
    Task<Result<Response.Authenticated>> LoginAsync(Query.Login request, CancellationToken cancellationToken = default);

}
