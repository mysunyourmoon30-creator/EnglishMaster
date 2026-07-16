using EnglishMaster.Domain.LearningGoals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class DailyStudyPlanConfiguration : IEntityTypeConfiguration<DailyStudyPlan>
{
    public void Configure(EntityTypeBuilder<DailyStudyPlan> builder)
    {
        builder.HasKey(plan => plan.Id);
        builder.Property(plan => plan.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasMany(plan => plan.Items).WithOne().HasForeignKey(item => item.DailyStudyPlanId).OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(DailyStudyPlan.Items))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(plan => new { plan.StudentProfileId, plan.PlanDate }).IsUnique();
        builder.HasIndex(plan => new { plan.StudentProfileId, plan.Status });
    }
}
