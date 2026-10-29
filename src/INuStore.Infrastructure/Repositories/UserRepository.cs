using INustore.UseCase.Abstractions.Repositories;
using INuStore.Domain.Entities.Identity;

namespace INuStore.Infrastructure.Repositories
{
    public sealed class UserRepository : RepositoryBase<User, int>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
