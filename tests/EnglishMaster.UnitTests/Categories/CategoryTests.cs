using EnglishMaster.Domain.Categories;

namespace EnglishMaster.UnitTests.Categories;

public sealed class CategoryTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var category = Category.Create("  Basics  ", "  Starter words  ", 10, now);

        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal("Basics", category.Name);
        Assert.Equal("basics", category.Slug);
        Assert.Equal("Starter words", category.Description);
        Assert.Equal(10, category.SortOrder);
        Assert.True(category.IsActive);
        Assert.Equal(now, category.CreatedAt);
        Assert.Equal(now, category.UpdatedAt);
    }

    [Theory]
    [InlineData("Common Words", "common-words")]
    [InlineData(" take_off ", "take-off")]
    [InlineData("can't", "cant")]
    public void GenerateSlugCreatesUrlFriendlyValue(string name, string expectedSlug)
    {
        var slug = Category.GenerateSlug(name);

        Assert.Equal(expectedSlug, slug);
    }
}
