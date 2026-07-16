using EnglishMaster.Domain.ContentQuality;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ContentQualityFindingConfiguration : IEntityTypeConfiguration<ContentQualityFinding>
{
    public void Configure(EntityTypeBuilder<ContentQualityFinding> builder)
    {
        builder.HasKey(finding => finding.Id);
        builder.Property(finding => finding.RuleCode).HasMaxLength(128).IsRequired();
        builder.Property(finding => finding.Severity).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(finding => finding.Message).HasMaxLength(1000).IsRequired();
        builder.Property(finding => finding.FieldName).HasMaxLength(128).IsRequired();
        builder.Property(finding => finding.Recommendation).HasMaxLength(1000).IsRequired();
        builder.HasIndex(finding => finding.RuleCode);
        builder.HasIndex(finding => finding.Severity);
        builder.HasIndex(finding => finding.IsResolved);
    }
}
