using EnglishMaster.Domain.ImportJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.ToTable("ImportJobs");
        builder.HasKey(job => job.Id);
        builder.Property(job => job.ImportType).HasMaxLength(64).IsRequired();
        builder.Property(job => job.Format).HasMaxLength(16).IsRequired();
        builder.Property(job => job.Status).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(job => job.FileName).HasMaxLength(260).IsRequired();
        builder.Property(job => job.OriginalFileName).HasMaxLength(260).IsRequired();
        builder.Property(job => job.RequestedBy).HasMaxLength(256);
        builder.Property(job => job.ErrorMessage).HasMaxLength(1000);
        builder.HasMany(job => job.Rows).WithOne().HasForeignKey(row => row.ImportJobId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(job => job.ImportType);
        builder.HasIndex(job => job.Format);
        builder.HasIndex(job => job.Status);
        builder.HasIndex(job => job.RequestedBy);
        builder.HasIndex(job => job.RequestedAt);
    }
}
