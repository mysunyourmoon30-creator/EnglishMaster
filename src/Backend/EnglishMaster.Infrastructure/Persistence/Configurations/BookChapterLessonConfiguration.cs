using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class BookChapterLessonConfiguration : IEntityTypeConfiguration<BookChapterLesson>
{
    public void Configure(EntityTypeBuilder<BookChapterLesson> builder)
    {
        builder.ToTable("BookChapterLessons");

        builder.HasKey(relation => relation.Id);

        builder.Property(relation => relation.Id)
            .ValueGeneratedNever();

        builder.Property(relation => relation.BookChapterId)
            .IsRequired();

        builder.Property(relation => relation.LessonId)
            .IsRequired();

        builder.Property(relation => relation.SortOrder)
            .IsRequired();

        builder.Property(relation => relation.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(relation => relation.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(relation => relation.BookChapterId);
        builder.HasIndex(relation => relation.LessonId);
        builder.HasIndex(relation => relation.SortOrder);
        builder.HasIndex(relation => new
            {
                relation.BookChapterId,
                relation.LessonId
            })
            .IsUnique();

        builder.HasOne<Lesson>()
            .WithMany()
            .HasForeignKey(relation => relation.LessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
