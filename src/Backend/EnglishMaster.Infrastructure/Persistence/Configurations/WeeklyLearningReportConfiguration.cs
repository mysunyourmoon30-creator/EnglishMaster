using EnglishMaster.Domain.LearningReports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class WeeklyLearningReportConfiguration : IEntityTypeConfiguration<WeeklyLearningReport>
{
    public void Configure(EntityTypeBuilder<WeeklyLearningReport> builder)
    {
        builder.HasKey(report => report.Id);
        builder.Property(report => report.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(report => report.SummaryText).HasMaxLength(1000);
        builder.HasMany(report => report.Insights).WithOne().HasForeignKey(insight => insight.WeeklyLearningReportId).OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(WeeklyLearningReport.Insights))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(report => new { report.StudentProfileId, report.WeekStartDate }).IsUnique();
        builder.HasIndex(report => new { report.StudentProfileId, report.GeneratedAt });
    }
}
