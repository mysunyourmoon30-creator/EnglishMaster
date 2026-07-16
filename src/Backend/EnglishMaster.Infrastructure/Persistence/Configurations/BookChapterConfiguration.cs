using EnglishMaster.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class BookChapterConfiguration : IEntityTypeConfiguration<BookChapter>
{
    public void Configure(EntityTypeBuilder<BookChapter> builder)
    {
        builder.ToTable("BookChapters");

        builder.HasKey(chapter => chapter.Id);

        builder.Property(chapter => chapter.Id)
            .ValueGeneratedNever();

        builder.Property(chapter => chapter.BookId)
            .IsRequired();

        builder.Property(chapter => chapter.Title)
            .HasMaxLength(BookChapterFieldLimits.Title)
            .IsRequired();

        builder.Property(chapter => chapter.Slug)
            .HasMaxLength(BookChapterFieldLimits.Slug)
            .IsRequired();

        builder.Property(chapter => chapter.Summary)
            .HasMaxLength(BookChapterFieldLimits.Summary)
            .IsRequired();

        builder.Property(chapter => chapter.ContentMarkdown)
            .HasMaxLength(BookChapterFieldLimits.ContentMarkdown)
            .IsRequired();

        builder.Property(chapter => chapter.SortOrder)
            .IsRequired();

        builder.Property(chapter => chapter.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(chapter => chapter.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(chapter => chapter.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(chapter => chapter.BookId);
        builder.HasIndex(chapter => chapter.SortOrder);
        builder.HasIndex(chapter => chapter.IsActive);

        builder.HasMany(chapter => chapter.Lessons)
            .WithOne()
            .HasForeignKey(relation => relation.BookChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(chapter => chapter.Lessons)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
