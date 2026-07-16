using EnglishMaster.Domain.LearningGoals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class DailyStudyPlanItemConfiguration : IEntityTypeConfiguration<DailyStudyPlanItem>
{
    public void Configure(EntityTypeBuilder<DailyStudyPlanItem> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ItemType).HasMaxLength(64).IsRequired();
        builder.Property(item => item.ContentType).HasMaxLength(64);
        builder.Property(item => item.Title).HasMaxLength(200).IsRequired();
        builder.Property(item => item.Url).HasMaxLength(500);
        builder.Property(item => item.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(item => new { item.DailyStudyPlanId, item.SortOrder });
    }
}
