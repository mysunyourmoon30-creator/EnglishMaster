using EnglishMaster.Domain.ContentQuality;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ContentQualityRuleConfiguration : IEntityTypeConfiguration<ContentQualityRule>
{
    public void Configure(EntityTypeBuilder<ContentQualityRule> builder)
    {
        builder.HasKey(rule => rule.Id);
        builder.Property(rule => rule.Code).HasMaxLength(128).IsRequired();
        builder.Property(rule => rule.Name).HasMaxLength(256).IsRequired();
        builder.Property(rule => rule.Description).HasMaxLength(1000).IsRequired();
        builder.Property(rule => rule.ContentType).HasMaxLength(64).IsRequired();
        builder.Property(rule => rule.Severity).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(rule => rule.Code).IsUnique();
        builder.HasIndex(rule => rule.ContentType);
        builder.HasIndex(rule => rule.Severity);
    }
}
