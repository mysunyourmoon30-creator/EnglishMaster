using EnglishMaster.Application.Features.Words.Queries;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Pronunciations;

public sealed class GetWordByIdWithPronunciationTests
{
    [Fact]
    public async Task HandleAsyncReturnsWordWithPronunciationSummary()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var words = new FakeWordRepository();
        var word = CreatePronunciationCommandHandlerTests.CreateWord("hello", now);
        words.Words.Add(word);
        var pronunciations = new FakePronunciationRepository();
        var pronunciation = CreatePronunciationCommandHandlerTests.CreatePronunciation(word.Id, now);
        pronunciations.Pronunciations.Add(pronunciation);
        var handler = new GetWordByIdQueryHandler(
            words,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            new FakeMediaRepository(),
            pronunciations);

        var result = await handler.HandleAsync(
            new GetWordByIdQuery(word.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(word.Id, result.Value!.Id);
        Assert.NotNull(result.Value.Pronunciation);
        Assert.Equal(pronunciation.Id, result.Value.Pronunciation!.Id);
        Assert.Equal("/hallo/", result.Value.Pronunciation.IpaUk);
    }
}
