using INuStore.Domain.Entities;
using INuStore.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INuStore.Infrastructure.Configurations;

internal sealed class ApplicationParameterConfiguration : IEntityTypeConfiguration<ApplicationParameter>
{
    public void Configure(EntityTypeBuilder<ApplicationParameter> builder)
    {
        builder.ToTable(TableNames.ApplicationParameter);
        builder.HasKey(parameter => parameter.Key);

        builder.Property(parameter => parameter.Key)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(256)
            .UseCollation("Latin1_General_100_CI_AS");

        builder.Property(parameter => parameter.Value)
            .IsRequired()
            .IsUnicode();
    }
}
