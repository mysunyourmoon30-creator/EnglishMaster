using EnglishMaster.Domain.Lessons;

namespace EnglishMaster.UnitTests.Lessons;

public sealed class LessonSectionTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var lessonId = Guid.NewGuid();
        var mediaId = Guid.NewGuid();

        var section = LessonSection.Create(
            lessonId,
            "  Vocabulary Warm-Up  ",
            "  ## Words to learn  ",
            SectionType.Vocabulary,
            mediaId,
            sortOrder: 1,
            now);

        Assert.NotEqual(Guid.Empty, section.Id);
        Assert.Equal(lessonId, section.LessonId);
        Assert.Equal("Vocabulary Warm-Up", section.Title);
        Assert.Equal("## Words to learn", section.ContentMarkdown);
        Assert.Equal(SectionType.Vocabulary, section.SectionType);
        Assert.Equal(mediaId, section.MediaId);
        Assert.Equal(1, section.SortOrder);
        Assert.True(section.IsActive);
        Assert.Equal(now, section.CreatedAt);
        Assert.Equal(now, section.UpdatedAt);
    }

    [Fact]
    public void CreateRequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() => LessonSection.Create(
            Guid.NewGuid(),
            " ",
            null,
            SectionType.Text,
            null,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("Title", exception.ParamName);
    }

    [Fact]
    public void ReorderUpdatesSortOrderAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var section = LessonSection.Create(
            Guid.NewGuid(),
            "Vocabulary Warm-Up",
            null,
            SectionType.Vocabulary,
            null,
            0,
            createdAt);

        var reorderedAt = createdAt.AddMinutes(5);
        section.Reorder(3, reorderedAt);

        Assert.Equal(3, section.SortOrder);
        Assert.Equal(reorderedAt, section.UpdatedAt);
    }
}
