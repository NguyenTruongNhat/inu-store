using INuStore.Domain.Entities.Identity;
using INuStore.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INuStore.Infrastructure.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(nameof(TableNames.Permissions));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(256);

        builder.HasIndex(x => x.Name).IsUnique();

        // Configure the relationship between Permission and RolePermission
        // A Permission can have many RolePermissions
        // if you dont configure in RolePermisionConfiguration, you need to configure it here

        //builder.HasMany(permission => permission.RolePermissions)
        //       .WithOne(link => link.Permission)
        //       .HasForeignKey(link => link.PermissionId)
        //       .OnDelete(DeleteBehavior.Cascade);

    }
}
