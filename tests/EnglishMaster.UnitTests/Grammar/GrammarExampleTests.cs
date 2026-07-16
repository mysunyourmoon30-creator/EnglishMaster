using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.UnitTests.Grammar;

public sealed class GrammarExampleTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var ruleId = Guid.NewGuid();

        var example = GrammarExample.Create(
            ruleId,
            "  She walks to school.  ",
            "  เธอเดินไปโรงเรียน  ",
            "  Add s after she  ",
            isCorrectExample: true,
            sortOrder: 1,
            now);

        Assert.NotEqual(Guid.Empty, example.Id);
        Assert.Equal(ruleId, example.GrammarRuleId);
        Assert.Equal("She walks to school.", example.ExampleEn);
        Assert.Equal("เธอเดินไปโรงเรียน", example.TranslationTh);
        Assert.Equal("Add s after she", example.ExplanationTh);
        Assert.True(example.IsCorrectExample);
        Assert.True(example.IsActive);
        Assert.Equal(now, example.CreatedAt);
        Assert.Equal(now, example.UpdatedAt);
    }

    [Fact]
    public void CreateRequiresExampleEn()
    {
        var exception = Assert.Throws<ArgumentException>(() => GrammarExample.Create(
            Guid.NewGuid(),
            " ",
            null,
            null,
            isCorrectExample: true,
            sortOrder: 0,
            DateTimeOffset.UtcNow));

        Assert.Equal("ExampleEn", exception.ParamName);
    }
}
