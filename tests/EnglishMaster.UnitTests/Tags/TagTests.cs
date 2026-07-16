using EnglishMaster.Domain.Tags;

namespace EnglishMaster.UnitTests.Tags;

public sealed class TagTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var tag = Tag.Create("  Travel  ", "  Trip vocabulary  ", now);

        Assert.NotEqual(Guid.Empty, tag.Id);
        Assert.Equal("Travel", tag.Name);
        Assert.Equal("travel", tag.Slug);
        Assert.Equal("Trip vocabulary", tag.Description);
        Assert.True(tag.IsActive);
        Assert.Equal(now, tag.CreatedAt);
        Assert.Equal(now, tag.UpdatedAt);
    }

    [Theory]
    [InlineData("Business English", "business-english")]
    [InlineData(" take_off ", "take-off")]
    [InlineData("can't", "cant")]
    public void GenerateSlugCreatesUrlFriendlyValue(string name, string expectedSlug)
    {
        var slug = Tag.GenerateSlug(name);

        Assert.Equal(expectedSlug, slug);
    }
}
