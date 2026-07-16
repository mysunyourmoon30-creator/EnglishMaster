using EnglishMaster.Domain.ContentQuality;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ContentQualityCheckConfiguration : IEntityTypeConfiguration<ContentQualityCheck>
{
    public void Configure(EntityTypeBuilder<ContentQualityCheck> builder)
    {
        builder.HasKey(check => check.Id);
        builder.Property(check => check.ContentType).HasMaxLength(64).IsRequired();
        builder.Property(check => check.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(check => check.CheckedBy).HasMaxLength(256).IsRequired();
        builder.HasMany(check => check.Findings)
            .WithOne()
            .HasForeignKey(finding => finding.ContentQualityCheckId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(check => check.Findings).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(check => check.ContentType);
        builder.HasIndex(check => check.ContentId);
        builder.HasIndex(check => check.Status);
        builder.HasIndex(check => check.CheckedAt);
    }
}
