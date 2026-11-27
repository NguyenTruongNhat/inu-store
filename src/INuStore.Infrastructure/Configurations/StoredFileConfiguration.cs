using INuStore.Domain.Entities.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INuStore.Infrastructure.Configurations;

internal sealed class StoredFileConfiguration
    : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("StoredFile");
        builder.HasKey(file => file.Id);
        builder.Property(file => file.StorageProvider).HasConversion<string>();
    }
}
