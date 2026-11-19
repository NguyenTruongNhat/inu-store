using INuStore.Contract.Abstractions.Share;
using INuStore.Contract.Service.V1.Auth;

namespace INuStore.UseCase.Abstractions.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(Command.Register request, CancellationToken cancellationToken = default);
    Task<Result<Response.Authenticated>> LoginAsync(Query.Login request, CancellationToken cancellationToken = default);
    Task<Result<Response.Authenticated>> RefreshTokenAsync(Query.Token request, CancellationToken cancellationToken = default);
    Task<Result> RevokeTokenAsync(Command.Revoke request, CancellationToken cancellationToken = default);

}
