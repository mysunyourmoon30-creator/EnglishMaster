using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.UnitTests.Grammar;

public sealed class GrammarTopicTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var topic = GrammarTopic.Create("  Present Simple  ", "  Daily routines  ", CefrLevel.A1, 2, now);

        Assert.NotEqual(Guid.Empty, topic.Id);
        Assert.Equal("Present Simple", topic.Title);
        Assert.Equal("present-simple", topic.Slug);
        Assert.Equal("Daily routines", topic.Summary);
        Assert.Equal(CefrLevel.A1, topic.CefrLevel);
        Assert.Equal(2, topic.SortOrder);
        Assert.True(topic.IsActive);
        Assert.Equal(now, topic.CreatedAt);
        Assert.Equal(now, topic.UpdatedAt);
    }

    [Theory]
    [InlineData("Present Simple", "present-simple")]
    [InlineData(" take_off grammar ", "take-off-grammar")]
    [InlineData("can't / won't", "cant-wont")]
    public void GenerateSlugCreatesUrlFriendlyValue(string title, string expectedSlug)
    {
        var slug = GrammarTopic.GenerateSlug(title);

        Assert.Equal(expectedSlug, slug);
    }

    [Fact]
    public void CreateRequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() => GrammarTopic.Create(
            " ",
            null,
            CefrLevel.A1,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("Title", exception.ParamName);
    }

    [Fact]
    public void ActivateAndDeactivateUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topic = GrammarTopic.Create("Present Simple", null, CefrLevel.A1, 0, createdAt);

        var deactivatedAt = createdAt.AddMinutes(5);
        topic.Deactivate(deactivatedAt);
        Assert.False(topic.IsActive);
        Assert.Equal(deactivatedAt, topic.UpdatedAt);

        var activatedAt = createdAt.AddMinutes(10);
        topic.Activate(activatedAt);
        Assert.True(topic.IsActive);
        Assert.Equal(activatedAt, topic.UpdatedAt);
    }
}
