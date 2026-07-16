using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class CourseLessonConfiguration : IEntityTypeConfiguration<CourseLesson>
{
    public void Configure(EntityTypeBuilder<CourseLesson> builder)
    {
        builder.ToTable("CourseLessons");

        builder.HasKey(courseLesson => courseLesson.Id);

        builder.Property(courseLesson => courseLesson.Id)
            .ValueGeneratedNever();

        builder.Property(courseLesson => courseLesson.CourseId)
            .IsRequired();

        builder.Property(courseLesson => courseLesson.LessonId)
            .IsRequired();

        builder.Property(courseLesson => courseLesson.SortOrder)
            .IsRequired();

        builder.Property(courseLesson => courseLesson.IsRequired)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(courseLesson => courseLesson.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(courseLesson => courseLesson.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(courseLesson => courseLesson.CourseId);
        builder.HasIndex(courseLesson => courseLesson.LessonId);
        builder.HasIndex(courseLesson => courseLesson.SortOrder);
        builder.HasIndex(courseLesson => new
            {
                courseLesson.CourseId,
                courseLesson.LessonId
            })
            .IsUnique();

        builder.HasOne<Lesson>()
            .WithMany()
            .HasForeignKey(courseLesson => courseLesson.LessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
