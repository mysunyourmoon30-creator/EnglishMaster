using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.UnitTests.Grammar;

public sealed class GrammarRuleTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var topicId = Guid.NewGuid();

        var rule = GrammarRule.Create(
            topicId,
            "  Positive Sentences  ",
            "  Subject + base verb  ",
            "  ใช้กับกิจวัตร  ",
            "  Used for routines  ",
            "  S + V1  ",
            "  Missing s for he/she/it  ",
            "  Add s for third person singular  ",
            1,
            now);

        Assert.NotEqual(Guid.Empty, rule.Id);
        Assert.Equal(topicId, rule.GrammarTopicId);
        Assert.Equal("Positive Sentences", rule.Title);
        Assert.Equal("positive-sentences", rule.Slug);
        Assert.Equal("Subject + base verb", rule.RuleText);
        Assert.Equal("ใช้กับกิจวัตร", rule.ExplanationTh);
        Assert.True(rule.IsActive);
        Assert.Equal(now, rule.CreatedAt);
        Assert.Equal(now, rule.UpdatedAt);
    }

    [Fact]
    public void CreateRequiresRuleText()
    {
        var exception = Assert.Throws<ArgumentException>(() => GrammarRule.Create(
            Guid.NewGuid(),
            "Positive Sentences",
            " ",
            null,
            null,
            null,
            null,
            null,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("RuleText", exception.ParamName);
    }

    [Fact]
    public void AddRelatedWordIgnoresDuplicates()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var rule = CreateRule(now);
        var wordId = Guid.NewGuid();

        rule.AddRelatedWord(wordId, now.AddMinutes(1));
        rule.AddRelatedWord(wordId, now.AddMinutes(2));

        Assert.Single(rule.RelatedWords);
        Assert.Equal(wordId, rule.RelatedWords.Single().WordId);
    }

    [Fact]
    public void RemoveRelatedWordReturnsFalseWhenRelationDoesNotExist()
    {
        var rule = CreateRule(DateTimeOffset.UtcNow);

        var removed = rule.RemoveRelatedWord(Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.False(removed);
    }

    private static GrammarRule CreateRule(DateTimeOffset now)
    {
        return GrammarRule.Create(
            Guid.NewGuid(),
            "Positive Sentences",
            "Subject + base verb",
            null,
            null,
            null,
            null,
            null,
            0,
            now);
    }
}
