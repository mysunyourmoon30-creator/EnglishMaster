using EnglishMaster.Application.Features.Pronunciations.Queries;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.Pronunciations;

public sealed class GetPronunciationByWordIdQueryHandlerTests
{
    [Fact]
    public async Task HandleAsyncReturnsPronunciationForWord()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var words = new FakeWordRepository();
        var word = CreatePronunciationCommandHandlerTests.CreateWord("hello", now);
        words.Words.Add(word);
        var pronunciations = new FakePronunciationRepository();
        var pronunciation = CreatePronunciationCommandHandlerTests.CreatePronunciation(word.Id, now);
        pronunciations.Pronunciations.Add(pronunciation);
        var handler = new GetPronunciationByWordIdQueryHandler(
            pronunciations,
            words,
            new FakeMediaRepository());

        var result = await handler.HandleAsync(
            new GetPronunciationByWordIdQuery(word.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(pronunciation.Id, result.Value!.Id);
        Assert.Equal(word.Id, result.Value.WordId);
        Assert.Equal("hello", result.Value.Word!.Text);
    }
}
