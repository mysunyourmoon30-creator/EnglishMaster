using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.UnitTests.Books;

public sealed class BookTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var categoryId = Guid.NewGuid();
        var coverMediaId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var book = Book.Create(
            "  Starter Book  ",
            "  First steps  ",
            "  A short summary  ",
            "  A longer description.  ",
            CefrLevel.A1,
            categoryId,
            coverMediaId,
            courseId,
            "  EnglishMaster Team  ",
            "  First  ",
            "  1.0  ",
            estimatedPages: 80,
            sortOrder: 2,
            now);

        Assert.NotEqual(Guid.Empty, book.Id);
        Assert.Equal("Starter Book", book.Title);
        Assert.Equal("starter-book", book.Slug);
        Assert.Equal("First steps", book.Subtitle);
        Assert.Equal("A short summary", book.Summary);
        Assert.Equal("A longer description.", book.Description);
        Assert.Equal(CefrLevel.A1, book.CefrLevel);
        Assert.Equal(categoryId, book.CategoryId);
        Assert.Equal(coverMediaId, book.CoverMediaId);
        Assert.Equal(courseId, book.CourseId);
        Assert.Equal("EnglishMaster Team", book.AuthorName);
        Assert.Equal("First", book.Edition);
        Assert.Equal("1.0", book.Version);
        Assert.Equal(80, book.EstimatedPages);
        Assert.Equal(2, book.SortOrder);
        Assert.False(book.IsPublished);
        Assert.True(book.IsActive);
        Assert.Equal(now, book.CreatedAt);
        Assert.Equal(now, book.UpdatedAt);
    }

    [Theory]
    [InlineData("Starter Book", "starter-book")]
    [InlineData(" A1 / First Steps ", "a1-first-steps")]
    [InlineData("teacher's guide", "teachers-guide")]
    public void GenerateSlugCreatesUrlFriendlyValue(string title, string expectedSlug)
    {
        var slug = Book.GenerateSlug(title);

        Assert.Equal(expectedSlug, slug);
    }

    [Fact]
    public void CreateRequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() => Book.Create(
            " ",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            0,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void EstimatedPagesMustNotBeNegative()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Book.Create(
            "Starter Book",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            -1,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("estimatedPages", exception.ParamName);
    }

    [Fact]
    public void PublishAndUnpublishUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var book = CreateBook(createdAt);

        var publishedAt = createdAt.AddMinutes(5);
        book.Publish(publishedAt);
        Assert.True(book.IsPublished);
        Assert.Equal(publishedAt, book.UpdatedAt);

        var unpublishedAt = createdAt.AddMinutes(10);
        book.Unpublish(unpublishedAt);
        Assert.False(book.IsPublished);
        Assert.Equal(unpublishedAt, book.UpdatedAt);
    }

    [Fact]
    public void ActivateAndDeactivateUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var book = CreateBook(createdAt);

        var deactivatedAt = createdAt.AddMinutes(5);
        book.Deactivate(deactivatedAt);
        Assert.False(book.IsActive);
        Assert.Equal(deactivatedAt, book.UpdatedAt);

        var activatedAt = createdAt.AddMinutes(10);
        book.Activate(activatedAt);
        Assert.True(book.IsActive);
        Assert.Equal(activatedAt, book.UpdatedAt);
    }

    [Fact]
    public void BookChapterRequiresTitleAndGeneratesSlug()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var chapter = BookChapter.Create(Guid.NewGuid(), " First Chapter ", " Summary ", " Content ", 3, now);

        Assert.Equal("First Chapter", chapter.Title);
        Assert.Equal("first-chapter", chapter.Slug);
        Assert.Equal(3, chapter.SortOrder);
        Assert.True(chapter.IsActive);
    }

    [Fact]
    public void AddLessonPreventsDuplicateLessonInsideChapter()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var chapter = BookChapter.Create(Guid.NewGuid(), "First Chapter", null, null, 0, now);
        var lessonId = Guid.NewGuid();

        var first = chapter.AddLesson(lessonId, 0, now.AddMinutes(1));
        var duplicate = chapter.AddLesson(lessonId, 1, now.AddMinutes(2));

        Assert.Single(chapter.Lessons);
        Assert.Equal(first.Id, duplicate.Id);
        Assert.Equal(lessonId, chapter.Lessons.Single().LessonId);
    }

    [Fact]
    public void BookChapterLessonReorderUpdatesSortOrderAndAuditField()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var relation = BookChapterLesson.Create(Guid.NewGuid(), Guid.NewGuid(), 0, now);

        var reorderedAt = now.AddMinutes(5);
        relation.Reorder(3, reorderedAt);

        Assert.Equal(3, relation.SortOrder);
        Assert.Equal(reorderedAt, relation.UpdatedAt);
    }

    private static Book CreateBook(DateTimeOffset now)
    {
        return Book.Create(
            "Starter Book",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            0,
            0,
            now);
    }
}
