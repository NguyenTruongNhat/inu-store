using INuStore.Domain.Entities.Identity;
using INuStore.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INuStore.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(50)
            .UseCollation("Latin1_General_100_CI_AS");

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(254)
            .UseCollation("Latin1_General_100_CI_AS");

        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email);

    }
}
