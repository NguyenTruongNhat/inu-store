using INustore.UseCase.Abstractions.Repositories;
using INuStore.Domain.Entities.Identity;

namespace INuStore.Infrastructure.Repositories
{
    public sealed class UserRepository : RepositoryBase<User, int>, IUserRepository
    {

        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await FindSingleAsync(user => user.UserName == userName, cancellationToken);
        }
    }
}
