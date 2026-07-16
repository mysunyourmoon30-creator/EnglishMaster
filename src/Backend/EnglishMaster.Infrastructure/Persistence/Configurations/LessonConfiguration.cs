using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.HasKey(lesson => lesson.Id);

        builder.Property(lesson => lesson.Title)
            .HasMaxLength(LessonFieldLimits.Title)
            .IsRequired();

        builder.Property(lesson => lesson.Slug)
            .HasMaxLength(LessonFieldLimits.Slug)
            .IsRequired();

        builder.Property(lesson => lesson.Summary)
            .HasMaxLength(LessonFieldLimits.Summary)
            .IsRequired();

        builder.Property(lesson => lesson.Description)
            .HasMaxLength(LessonFieldLimits.Description)
            .IsRequired();

        builder.Property(lesson => lesson.CefrLevel)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(lesson => lesson.CategoryId);

        builder.Property(lesson => lesson.ThumbnailMediaId);

        builder.Property(lesson => lesson.EstimatedMinutes)
            .IsRequired();

        builder.Property(lesson => lesson.SortOrder)
            .IsRequired();

        builder.Property(lesson => lesson.IsPublished)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(lesson => lesson.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(lesson => lesson.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(lesson => lesson.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(lesson => lesson.Title);
        builder.HasIndex(lesson => lesson.Slug)
            .IsUnique();
        builder.HasIndex(lesson => lesson.CefrLevel);
        builder.HasIndex(lesson => lesson.CategoryId);
        builder.HasIndex(lesson => lesson.IsPublished);
        builder.HasIndex(lesson => lesson.IsActive);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(lesson => lesson.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(lesson => lesson.ThumbnailMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(lesson => lesson.Sections)
            .WithOne()
            .HasForeignKey(section => section.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(lesson => lesson.Words)
            .WithOne()
            .HasForeignKey(lessonWord => lessonWord.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(lesson => lesson.GrammarRules)
            .WithOne()
            .HasForeignKey(lessonGrammarRule => lessonGrammarRule.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(lesson => lesson.Sections)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(lesson => lesson.Words)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(lesson => lesson.GrammarRules)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
