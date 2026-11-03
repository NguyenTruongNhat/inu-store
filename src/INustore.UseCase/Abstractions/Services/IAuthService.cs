using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Auth;

namespace INustore.UseCase.Abstractions.Services
{
    public interface IAuthService
    {
        public Task<Result<Response.Authenticated>> LoginAsync(Query.Login request, CancellationToken cancellationToken = default);

    }
}
