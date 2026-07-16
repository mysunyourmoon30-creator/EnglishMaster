using EnglishMaster.Domain.ImportJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ImportValidationErrorConfiguration : IEntityTypeConfiguration<ImportValidationError>
{
    public void Configure(EntityTypeBuilder<ImportValidationError> builder)
    {
        builder.ToTable("ImportValidationErrors");
        builder.HasKey(error => error.Id);
        builder.Property(error => error.FieldName).HasMaxLength(128);
        builder.Property(error => error.ErrorCode).HasMaxLength(128).IsRequired();
        builder.Property(error => error.ErrorMessage).HasMaxLength(1000).IsRequired();
        builder.Property(error => error.Severity).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.HasIndex(error => error.ImportJobRowId);
        builder.HasIndex(error => error.Severity);
    }
}
