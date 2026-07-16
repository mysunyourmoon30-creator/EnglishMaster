using EnglishMaster.Application.Features.MinimalPairs.Commands;
using EnglishMaster.Domain.Media;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.Pronunciations;
using EnglishMaster.UnitTests.TestDoubles;

namespace EnglishMaster.UnitTests.MinimalPairs;

public sealed class AddMinimalPairCommandHandlerTests
{
    [Fact]
    public async Task HandleAsyncAddsMinimalPair()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var pronunciations = new FakePronunciationRepository();
        var pronunciation = CreatePronunciationCommandHandlerTests.CreatePronunciation(Guid.NewGuid(), now);
        pronunciations.Pronunciations.Add(pronunciation);
        var media = new FakeMediaRepository();
        var audio = CreatePronunciationCommandHandlerTests.CreateMedia(
            "ship.mp3",
            MediaType.Audio,
            "audio/mpeg",
            now);
        media.Media.Add(audio);
        var repository = new FakeMinimalPairRepository();
        var handler = new AddMinimalPairCommandHandler(
            repository,
            pronunciations,
            media,
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new AddMinimalPairCommand(
                pronunciation.Id,
                "ship",
                "/ship/",
                "ship-reading",
                "short i",
                audio.Id,
                1),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.MinimalPairs);
        Assert.Equal(pronunciation.Id, result.Value!.PronunciationId);
        Assert.Equal(audio.Id, result.Value.AudioMediaId);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenPairTextIsMissing()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var pronunciations = new FakePronunciationRepository();
        var pronunciation = CreatePronunciationCommandHandlerTests.CreatePronunciation(Guid.NewGuid(), now);
        pronunciations.Pronunciations.Add(pronunciation);
        var repository = new FakeMinimalPairRepository();
        var handler = new AddMinimalPairCommandHandler(
            repository,
            pronunciations,
            new FakeMediaRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new AddMinimalPairCommand(pronunciation.Id, string.Empty, null, null, null, null),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Empty(repository.MinimalPairs);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenPronunciationIsInactive()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var pronunciations = new FakePronunciationRepository();
        var pronunciation = CreatePronunciationCommandHandlerTests.CreatePronunciation(Guid.NewGuid(), now);
        pronunciation.Deactivate(now.AddMinutes(1));
        pronunciations.Pronunciations.Add(pronunciation);
        var repository = new FakeMinimalPairRepository();
        var handler = new AddMinimalPairCommandHandler(
            repository,
            pronunciations,
            new FakeMediaRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new AddMinimalPairCommand(pronunciation.Id, "ship", "/ship/", null, null, null),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == nameof(AddMinimalPairCommand.PronunciationId));
        Assert.Empty(repository.MinimalPairs);
    }
}
