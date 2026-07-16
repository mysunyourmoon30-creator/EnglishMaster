using EnglishMaster.Domain.Practice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class PracticeItemConfiguration : IEntityTypeConfiguration<PracticeItem>
{
    public void Configure(EntityTypeBuilder<PracticeItem> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ContentType).HasMaxLength(64).IsRequired();
        builder.Property(item => item.PracticeType).HasMaxLength(64).IsRequired();
        builder.Property(item => item.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(item => new { item.StudentProfileId, item.ContentType, item.ContentId, item.PracticeType }).IsUnique();
        builder.HasIndex(item => new { item.StudentProfileId, item.Status, item.DueAt });
    }
}

