using EnglishMaster.Domain.Pronunciations;

namespace EnglishMaster.UnitTests.MinimalPairs;

public sealed class MinimalPairTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var pronunciationId = Guid.NewGuid();
        var audioMediaId = Guid.NewGuid();

        var minimalPair = MinimalPair.Create(
            pronunciationId,
            " ship ",
            " /ship/ ",
            " ship-reading ",
            " short i sound ",
            audioMediaId,
            2,
            now);

        Assert.NotEqual(Guid.Empty, minimalPair.Id);
        Assert.Equal(pronunciationId, minimalPair.PronunciationId);
        Assert.Equal("ship", minimalPair.PairWordText);
        Assert.Equal("/ship/", minimalPair.PairIpa);
        Assert.Equal(audioMediaId, minimalPair.AudioMediaId);
        Assert.Equal(2, minimalPair.SortOrder);
        Assert.True(minimalPair.IsActive);
        Assert.Equal(now, minimalPair.CreatedAt);
        Assert.Equal(now, minimalPair.UpdatedAt);
    }

    [Fact]
    public void CreateRequiresPairWordText()
    {
        var exception = Assert.Throws<ArgumentException>(() => MinimalPair.Create(
            Guid.NewGuid(),
            string.Empty,
            null,
            null,
            null,
            null,
            0,
            DateTimeOffset.UtcNow));

        Assert.Equal("PairWordText", exception.ParamName);
    }

    [Fact]
    public void CreateRejectsNegativeSortOrder()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => MinimalPair.Create(
            Guid.NewGuid(),
            "ship",
            null,
            null,
            null,
            null,
            -1,
            DateTimeOffset.UtcNow));

        Assert.Equal("sortOrder", exception.ParamName);
    }
}
