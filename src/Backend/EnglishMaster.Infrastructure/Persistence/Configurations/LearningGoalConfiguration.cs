using EnglishMaster.Domain.LearningGoals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class LearningGoalConfiguration : IEntityTypeConfiguration<LearningGoal>
{
    public void Configure(EntityTypeBuilder<LearningGoal> builder)
    {
        builder.HasKey(goal => goal.Id);
        builder.Property(goal => goal.GoalType).HasMaxLength(64).IsRequired();
        builder.Property(goal => goal.Title).HasMaxLength(160).IsRequired();
        builder.Property(goal => goal.Description).HasMaxLength(1000);
        builder.Property(goal => goal.Unit).HasMaxLength(40);
        builder.Property(goal => goal.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(goal => new { goal.StudentProfileId, goal.Status });
        builder.HasIndex(goal => new { goal.StudentProfileId, goal.GoalType });
    }
}
