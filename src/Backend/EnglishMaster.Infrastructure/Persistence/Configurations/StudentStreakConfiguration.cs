using EnglishMaster.Domain.Motivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class StudentStreakConfiguration : IEntityTypeConfiguration<StudentStreak>
{
    public void Configure(EntityTypeBuilder<StudentStreak> builder)
    {
        builder.HasKey(streak => streak.Id);
        builder.HasIndex(streak => streak.StudentProfileId).IsUnique();
    }
}
