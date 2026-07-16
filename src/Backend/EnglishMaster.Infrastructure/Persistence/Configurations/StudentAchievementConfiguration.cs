using EnglishMaster.Domain.Motivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class StudentAchievementConfiguration : IEntityTypeConfiguration<StudentAchievement>
{
    public void Configure(EntityTypeBuilder<StudentAchievement> builder)
    {
        builder.HasKey(achievement => achievement.Id);
        builder.Property(achievement => achievement.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(achievement => new { achievement.StudentProfileId, achievement.AchievementDefinitionId }).IsUnique();
        builder.HasIndex(achievement => achievement.StudentProfileId);
        builder.HasIndex(achievement => achievement.Status);
        builder.HasIndex(achievement => achievement.EarnedAt);
    }
}
