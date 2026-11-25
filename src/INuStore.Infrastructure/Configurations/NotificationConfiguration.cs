using INuStore.Domain.Entities;
using INuStore.Domain.Entities.Identity;
using INuStore.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INuStore.Infrastructure.Configurations;

internal sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable(TableNames.Notifications);
        builder.HasKey(notification => notification.Id);

        builder.Property(notification => notification.Recipient)
            .IsRequired();

        builder.Property(notification => notification.Body)
            .IsRequired();

        builder.Property(notification => notification.ProviderMessageId)
            .HasMaxLength(100);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(notification => notification.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
