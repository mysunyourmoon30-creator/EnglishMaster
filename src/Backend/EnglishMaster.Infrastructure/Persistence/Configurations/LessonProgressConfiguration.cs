using EnglishMaster.Domain.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.HasKey(progress => progress.Id);
        builder.Property(progress => progress.Status).HasConversion<string>().HasMaxLength(32);
        builder.HasIndex(progress => new { progress.UserId, progress.LessonId }).IsUnique();
        builder.HasIndex(progress => new { progress.UserId, progress.Status, progress.LastAccessedAt });
    }
}

