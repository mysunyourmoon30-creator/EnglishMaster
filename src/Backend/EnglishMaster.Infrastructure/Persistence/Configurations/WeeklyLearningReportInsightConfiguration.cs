using EnglishMaster.Domain.LearningReports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class WeeklyLearningReportInsightConfiguration : IEntityTypeConfiguration<WeeklyLearningReportInsight>
{
    public void Configure(EntityTypeBuilder<WeeklyLearningReportInsight> builder)
    {
        builder.HasKey(insight => insight.Id);
        builder.Property(insight => insight.InsightType).HasMaxLength(64).IsRequired();
        builder.Property(insight => insight.Severity).HasMaxLength(32).IsRequired();
        builder.Property(insight => insight.Message).HasMaxLength(300).IsRequired();
        builder.Property(insight => insight.Recommendation).HasMaxLength(500);
        builder.HasIndex(insight => new { insight.WeeklyLearningReportId, insight.SortOrder });
    }
}
