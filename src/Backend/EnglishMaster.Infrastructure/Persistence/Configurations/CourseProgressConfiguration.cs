using EnglishMaster.Domain.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class CourseProgressConfiguration : IEntityTypeConfiguration<CourseProgress>
{
    public void Configure(EntityTypeBuilder<CourseProgress> builder)
    {
        builder.HasKey(progress => progress.Id);
        builder.Property(progress => progress.Status).HasConversion<string>().HasMaxLength(32);
        builder.HasIndex(progress => new { progress.UserId, progress.CourseId }).IsUnique();
        builder.HasIndex(progress => new { progress.UserId, progress.Status, progress.LastAccessedAt });
    }
}

