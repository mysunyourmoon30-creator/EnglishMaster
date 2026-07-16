using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(course => course.Id);

        builder.Property(course => course.Title)
            .HasMaxLength(CourseFieldLimits.Title)
            .IsRequired();

        builder.Property(course => course.Slug)
            .HasMaxLength(CourseFieldLimits.Slug)
            .IsRequired();

        builder.Property(course => course.Summary)
            .HasMaxLength(CourseFieldLimits.Summary)
            .IsRequired();

        builder.Property(course => course.Description)
            .HasMaxLength(CourseFieldLimits.Description)
            .IsRequired();

        builder.Property(course => course.CefrLevel)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(course => course.CategoryId);

        builder.Property(course => course.ThumbnailMediaId);

        builder.Property(course => course.EstimatedMinutes)
            .IsRequired();

        builder.Property(course => course.SortOrder)
            .IsRequired();

        builder.Property(course => course.IsPublished)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(course => course.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(course => course.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(course => course.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(course => course.Title);
        builder.HasIndex(course => course.Slug)
            .IsUnique();
        builder.HasIndex(course => course.CefrLevel);
        builder.HasIndex(course => course.CategoryId);
        builder.HasIndex(course => course.IsPublished);
        builder.HasIndex(course => course.IsActive);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(course => course.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(course => course.ThumbnailMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(course => course.Lessons)
            .WithOne()
            .HasForeignKey(courseLesson => courseLesson.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(course => course.Lessons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
