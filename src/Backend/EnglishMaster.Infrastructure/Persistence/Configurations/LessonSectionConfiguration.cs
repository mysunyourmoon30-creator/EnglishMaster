using EnglishMaster.Domain.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class LessonSectionConfiguration : IEntityTypeConfiguration<LessonSection>
{
    public void Configure(EntityTypeBuilder<LessonSection> builder)
    {
        builder.ToTable("LessonSections");

        builder.HasKey(lessonSection => lessonSection.Id);

        builder.Property(lessonSection => lessonSection.LessonId)
            .IsRequired();

        builder.Property(lessonSection => lessonSection.Title)
            .HasMaxLength(LessonSectionFieldLimits.Title)
            .IsRequired();

        builder.Property(lessonSection => lessonSection.ContentMarkdown)
            .HasMaxLength(LessonSectionFieldLimits.ContentMarkdown)
            .IsRequired();

        builder.Property(lessonSection => lessonSection.SectionType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(lessonSection => lessonSection.MediaId);

        builder.Property(lessonSection => lessonSection.SortOrder)
            .IsRequired();

        builder.Property(lessonSection => lessonSection.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(lessonSection => lessonSection.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(lessonSection => lessonSection.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(lessonSection => lessonSection.LessonId);
        builder.HasIndex(lessonSection => lessonSection.SortOrder);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(lessonSection => lessonSection.MediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
