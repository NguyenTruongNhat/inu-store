using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Abstractions.Repositories;

public interface IUserRepository : IRepositoryBase<User, int>
{
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<int> CountByEmailForUpdateAsync(string email, int? excludedUserId, CancellationToken cancellationToken = default);

    Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithRolesAsync(int userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int userId, CancellationToken cancellationToken = default);

    void AddUserRole(UserRole userRole);

    void RemoveUserRole(UserRole userRole);
}
