using EnglishMaster.Domain.Words;
using MediaEntity = EnglishMaster.Domain.Media.Media;
using MediaType = EnglishMaster.Domain.Media.MediaType;

namespace EnglishMaster.UnitTests.Words;

public sealed class WordTests
{
    [Fact]
    public void CreateNormalizesInputAndSetsAuditFields()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

        var word = Word.Create(
            "  hello  ",
            "  /he'lo/ ",
            " /he'lo/ ",
            " heh-lo ",
            " สวัสดี ",
            " greeting ",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            " Hello there. ",
            " สวัสดี ",
            now);

        Assert.NotEqual(Guid.Empty, word.Id);
        Assert.Equal("hello", word.Text);
        Assert.Equal("hello", word.Slug);
        Assert.Equal("/he'lo/", word.IpaUk);
        Assert.Equal("greeting", word.MeaningEn);
        Assert.True(word.IsActive);
        Assert.Equal(now, word.CreatedAt);
        Assert.Equal(now, word.UpdatedAt);
    }

    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData(" take_off ", "take-off")]
    [InlineData("well-known", "well-known")]
    [InlineData("can't", "cant")]
    public void GenerateSlugCreatesUrlFriendlyValue(string text, string expectedSlug)
    {
        var slug = Word.GenerateSlug(text);

        Assert.Equal(expectedSlug, slug);
    }

    [Fact]
    public void GenerateSlugRequiresLetterOrDigit()
    {
        var exception = Assert.Throws<ArgumentException>(() => Word.GenerateSlug(" !!! "));

        Assert.Equal("text", exception.ParamName);
    }

    [Fact]
    public void DeactivateMarksWordInactiveAndUpdatesAuditField()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var deletedAt = createdAt.AddHours(1);
        var word = Word.Create(
            "hello",
            string.Empty,
            string.Empty,
            string.Empty,
            "สวัสดี",
            "greeting",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            createdAt);

        word.Deactivate(deletedAt);

        Assert.False(word.IsActive);
        Assert.Equal(deletedAt, word.UpdatedAt);
    }

    [Fact]
    public void CreateCanAssignCategoryAndTags()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var categoryId = Guid.NewGuid();
        var tagId = Guid.NewGuid();

        var word = Word.Create(
            "airport",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "airport",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            categoryId,
            [tagId],
            now);

        Assert.Equal(categoryId, word.CategoryId);
        var wordTag = Assert.Single(word.Tags);
        Assert.Equal(tagId, wordTag.TagId);
    }

    [Fact]
    public void CreateCanAssignImageAndAudioMedia()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var imageMediaId = Guid.NewGuid();
        var audioMediaId = Guid.NewGuid();

        var word = Word.Create(
            "listen",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "listen",
            PartOfSpeech.Verb,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            categoryId: null,
            tagIds: [],
            imageMediaId,
            audioMediaId,
            now);

        Assert.Equal(imageMediaId, word.ImageMediaId);
        Assert.Equal(audioMediaId, word.AudioMediaId);
    }

    [Fact]
    public void SetTagsRemovesDuplicates()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var tagId = Guid.NewGuid();
        var word = Word.Create(
            "airport",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "airport",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);

        word.SetTags([tagId, tagId], now.AddMinutes(1));

        var wordTag = Assert.Single(word.Tags);
        Assert.Equal(tagId, wordTag.TagId);
    }

    [Fact]
    public void SetMediaRejectsEmptyIds()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var word = Word.Create(
            "airport",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "airport",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);

        Assert.Throws<ArgumentException>(() => word.SetImageMedia(Guid.Empty, now));
        Assert.Throws<ArgumentException>(() => word.SetAudioMedia(Guid.Empty, now));
    }
}
