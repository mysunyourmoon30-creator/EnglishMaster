using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .HasMaxLength(BookFieldLimits.Title)
            .IsRequired();

        builder.Property(book => book.Slug)
            .HasMaxLength(BookFieldLimits.Slug)
            .IsRequired();

        builder.Property(book => book.Subtitle)
            .HasMaxLength(BookFieldLimits.Subtitle)
            .IsRequired();

        builder.Property(book => book.Summary)
            .HasMaxLength(BookFieldLimits.Summary)
            .IsRequired();

        builder.Property(book => book.Description)
            .HasMaxLength(BookFieldLimits.Description)
            .IsRequired();

        builder.Property(book => book.CefrLevel)
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(book => book.CategoryId);

        builder.Property(book => book.CoverMediaId);

        builder.Property(book => book.CourseId);

        builder.Property(book => book.AuthorName)
            .HasMaxLength(BookFieldLimits.AuthorName)
            .IsRequired();

        builder.Property(book => book.Edition)
            .HasMaxLength(BookFieldLimits.Edition)
            .IsRequired();

        builder.Property(book => book.Version)
            .HasMaxLength(BookFieldLimits.Version)
            .IsRequired();

        builder.Property(book => book.EstimatedPages)
            .IsRequired();

        builder.Property(book => book.SortOrder)
            .IsRequired();

        builder.Property(book => book.IsPublished)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(book => book.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(book => book.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(book => book.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(book => book.Title);
        builder.HasIndex(book => book.Slug)
            .IsUnique();
        builder.HasIndex(book => book.CefrLevel);
        builder.HasIndex(book => book.CategoryId);
        builder.HasIndex(book => book.CourseId);
        builder.HasIndex(book => book.IsPublished);
        builder.HasIndex(book => book.IsActive);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(book => book.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(book => book.CoverMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(book => book.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(book => book.Chapters)
            .WithOne()
            .HasForeignKey(chapter => chapter.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(book => book.Chapters)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
