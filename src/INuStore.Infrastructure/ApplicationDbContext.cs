using INuStore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure;
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
        => builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

    public DbSet<User> Users { get; set; }

}
