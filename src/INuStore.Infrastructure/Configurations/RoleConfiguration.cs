using INuStore.Domain.Entities.Identity;
using INuStore.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INuStore.Infrastructure.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(TableNames.Roles);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(256);

        // configure the relationship between Role and RolePermission
        // A Role can have many RolePermissions
        // if you dont configure in RolePermisionConfiguration, you need to configure it here
        //builder.HasMany(role => role.RolePermissions)
        //       .WithOne(link => link.Role)
        //       .HasForeignKey(link => link.RoleId)
        //       .OnDelete(DeleteBehavior.Cascade);
    }
}
