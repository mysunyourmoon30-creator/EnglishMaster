using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.UnitTests.Lessons;

public sealed class LessonTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var categoryId = Guid.NewGuid();
        var thumbnailMediaId = Guid.NewGuid();

        var lesson = Lesson.Create(
            "  Daily Routines  ",
            "  Learn to talk about your day  ",
            "  A longer description of the lesson.  ",
            CefrLevel.A1,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes: 15,
            sortOrder: 2,
            now);

        Assert.NotEqual(Guid.Empty, lesson.Id);
        Assert.Equal("Daily Routines", lesson.Title);
        Assert.Equal("daily-routines", lesson.Slug);
        Assert.Equal("Learn to talk about your day", lesson.Summary);
        Assert.Equal("A longer description of the lesson.", lesson.Description);
        Assert.Equal(CefrLevel.A1, lesson.CefrLevel);
        Assert.Equal(categoryId, lesson.CategoryId);
        Assert.Equal(thumbnailMediaId, lesson.ThumbnailMediaId);
        Assert.Equal(15, lesson.EstimatedMinutes);
        Assert.Equal(2, lesson.SortOrder);
        Assert.False(lesson.IsPublished);
        Assert.True(lesson.IsActive);
        Assert.Equal(now, lesson.CreatedAt);
        Assert.Equal(now, lesson.UpdatedAt);
    }

    [Theory]
    [InlineData("Daily Routines", "daily-routines")]
    [InlineData(" take_off grammar ", "take-off-grammar")]
    [InlineData("can't / won't", "cant-wont")]
    public void GenerateSlugCreatesUrlFriendlyValue(string title, string expectedSlug)
    {
        var slug = Lesson.GenerateSlug(title);

        Assert.Equal(expectedSlug, slug);
    }

    [Fact]
    public void CreateRequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() => Lesson.Create(
            " ",
            null,
            null,
            null,
            null,
            null,
            0,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("Title", exception.ParamName);
    }

    [Fact]
    public void ActivateAndDeactivateUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lesson = CreateLesson(createdAt);

        var deactivatedAt = createdAt.AddMinutes(5);
        lesson.Deactivate(deactivatedAt);
        Assert.False(lesson.IsActive);
        Assert.Equal(deactivatedAt, lesson.UpdatedAt);

        var activatedAt = createdAt.AddMinutes(10);
        lesson.Activate(activatedAt);
        Assert.True(lesson.IsActive);
        Assert.Equal(activatedAt, lesson.UpdatedAt);
    }

    [Fact]
    public void PublishAndUnpublishUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lesson = CreateLesson(createdAt);
        Assert.False(lesson.IsPublished);

        var publishedAt = createdAt.AddMinutes(5);
        lesson.Publish(publishedAt);
        Assert.True(lesson.IsPublished);
        Assert.Equal(publishedAt, lesson.UpdatedAt);

        var unpublishedAt = createdAt.AddMinutes(10);
        lesson.Unpublish(unpublishedAt);
        Assert.False(lesson.IsPublished);
        Assert.Equal(unpublishedAt, lesson.UpdatedAt);
    }

    [Fact]
    public void AddWordIgnoresDuplicates()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lesson = CreateLesson(now);
        var wordId = Guid.NewGuid();

        lesson.AddWord(wordId, 0, now.AddMinutes(1));
        lesson.AddWord(wordId, 0, now.AddMinutes(2));

        Assert.Single(lesson.Words);
        Assert.Equal(wordId, lesson.Words.Single().WordId);
    }

    [Fact]
    public void RemoveWordReturnsFalseWhenRelationDoesNotExist()
    {
        var lesson = CreateLesson(DateTimeOffset.UtcNow);

        var removed = lesson.RemoveWord(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.False(removed);
    }

    [Fact]
    public void AddGrammarRuleIgnoresDuplicates()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lesson = CreateLesson(now);
        var grammarRuleId = Guid.NewGuid();

        lesson.AddGrammarRule(grammarRuleId, 0, now.AddMinutes(1));
        lesson.AddGrammarRule(grammarRuleId, 0, now.AddMinutes(2));

        Assert.Single(lesson.GrammarRules);
        Assert.Equal(grammarRuleId, lesson.GrammarRules.Single().GrammarRuleId);
    }

    [Fact]
    public void RemoveGrammarRuleReturnsFalseWhenRelationDoesNotExist()
    {
        var lesson = CreateLesson(DateTimeOffset.UtcNow);

        var removed = lesson.RemoveGrammarRule(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.False(removed);
    }

    private static Lesson CreateLesson(DateTimeOffset now)
    {
        return Lesson.Create(
            "Daily Routines",
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
