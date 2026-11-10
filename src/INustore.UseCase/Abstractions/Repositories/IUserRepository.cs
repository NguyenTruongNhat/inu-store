using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface IUserRepository : IRepositoryBase<User, int>
{
    Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithRolesAsync(int userId, CancellationToken cancellationToken = default);

    void AddUserRole(UserRole userRole);

    void RemoveUserRole(UserRole userRole);
}
