using EnglishMaster.Domain.Pronunciations;

namespace EnglishMaster.UnitTests.Pronunciations;

public sealed class PronunciationTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var wordId = Guid.NewGuid();

        var pronunciation = Pronunciation.Create(
            wordId,
            " /hallo/ ",
            " /hello/ ",
            " heh-lo ",
            " hel-lo ",
            " first syllable ",
            " lips open ",
            " tongue low ",
            " avoid silent h ",
            " practice slowly ",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            now);

        Assert.NotEqual(Guid.Empty, pronunciation.Id);
        Assert.Equal(wordId, pronunciation.WordId);
        Assert.Equal("/hallo/", pronunciation.IpaUk);
        Assert.Equal("/hello/", pronunciation.IpaUs);
        Assert.Equal("heh-lo", pronunciation.ThaiReading);
        Assert.True(pronunciation.IsActive);
        Assert.Equal(now, pronunciation.CreatedAt);
        Assert.Equal(now, pronunciation.UpdatedAt);
    }

    [Fact]
    public void CreateRequiresWordId()
    {
        var exception = Assert.Throws<ArgumentException>(() => Pronunciation.Create(
            Guid.Empty,
            "/hallo/",
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
            null,
            DateTimeOffset.UtcNow));

        Assert.Equal("wordId", exception.ParamName);
    }

    [Fact]
    public void CreateRequiresIpaUkOrIpaUs()
    {
        var exception = Assert.Throws<ArgumentException>(() => Pronunciation.Create(
            Guid.NewGuid(),
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
            null,
            null,
            DateTimeOffset.UtcNow));

        Assert.Equal("ipaUk", exception.ParamName);
    }

    [Fact]
    public void ActivateAndDeactivateUpdateStatusAndAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var pronunciation = Pronunciation.Create(
            Guid.NewGuid(),
            "/hallo/",
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
            null,
            createdAt);

        var deactivatedAt = createdAt.AddMinutes(5);
        pronunciation.Deactivate(deactivatedAt);
        Assert.False(pronunciation.IsActive);
        Assert.Equal(deactivatedAt, pronunciation.UpdatedAt);

        var activatedAt = createdAt.AddMinutes(10);
        pronunciation.Activate(activatedAt);
        Assert.True(pronunciation.IsActive);
        Assert.Equal(activatedAt, pronunciation.UpdatedAt);
    }
}
