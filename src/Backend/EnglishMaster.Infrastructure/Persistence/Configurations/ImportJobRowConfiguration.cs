using EnglishMaster.Domain.ImportJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ImportJobRowConfiguration : IEntityTypeConfiguration<ImportJobRow>
{
    public void Configure(EntityTypeBuilder<ImportJobRow> builder)
    {
        builder.ToTable("ImportJobRows");
        builder.HasKey(row => row.Id);
        builder.Property(row => row.RawDataJson).HasMaxLength(8000).IsRequired();
        builder.Property(row => row.ParsedDataJson).HasMaxLength(8000);
        builder.Property(row => row.Status).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(row => row.ErrorMessage).HasMaxLength(1000);
        builder.Property(row => row.CreatedEntityType).HasMaxLength(64);
        builder.Property(row => row.UpdatedEntityType).HasMaxLength(64);
        builder.HasMany(row => row.ValidationErrors).WithOne().HasForeignKey(error => error.ImportJobRowId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(row => row.ImportJobId);
        builder.HasIndex(row => row.RowNumber);
        builder.HasIndex(row => row.Status);
    }
}
