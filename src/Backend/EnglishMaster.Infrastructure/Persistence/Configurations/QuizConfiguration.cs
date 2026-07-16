using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");

        builder.HasKey(quiz => quiz.Id);

        builder.Property(quiz => quiz.Title)
            .HasMaxLength(QuizFieldLimits.Title)
            .IsRequired();

        builder.Property(quiz => quiz.Slug)
            .HasMaxLength(QuizFieldLimits.Slug)
            .IsRequired();

        builder.Property(quiz => quiz.Summary)
            .HasMaxLength(QuizFieldLimits.Summary)
            .IsRequired();

        builder.Property(quiz => quiz.Description)
            .HasMaxLength(QuizFieldLimits.Description)
            .IsRequired();

        builder.Property(quiz => quiz.CefrLevel)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(quiz => quiz.TimeLimitMinutes)
            .IsRequired();

        builder.Property(quiz => quiz.PassingScore)
            .IsRequired();

        builder.Property(quiz => quiz.SortOrder)
            .IsRequired();

        builder.Property(quiz => quiz.IsPublished)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(quiz => quiz.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(quiz => quiz.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(quiz => quiz.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(quiz => quiz.Title);
        builder.HasIndex(quiz => quiz.Slug)
            .IsUnique();
        builder.HasIndex(quiz => quiz.CefrLevel);
        builder.HasIndex(quiz => quiz.CategoryId);
        builder.HasIndex(quiz => quiz.LessonId);
        builder.HasIndex(quiz => quiz.CourseId);
        builder.HasIndex(quiz => quiz.BookId);
        builder.HasIndex(quiz => quiz.IsPublished);
        builder.HasIndex(quiz => quiz.IsActive);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(quiz => quiz.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Lesson>()
            .WithMany()
            .HasForeignKey(quiz => quiz.LessonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(quiz => quiz.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Book>()
            .WithMany()
            .HasForeignKey(quiz => quiz.BookId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(quiz => quiz.Questions)
            .WithOne()
            .HasForeignKey(question => question.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(quiz => quiz.Questions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
