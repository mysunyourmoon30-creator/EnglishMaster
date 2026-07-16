using EnglishMaster.Application.Features.Pronunciations.Commands;
using EnglishMaster.Domain.Media;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.UnitTests.Pronunciations;

public sealed class CreatePronunciationCommandHandlerTests
{
    [Fact]
    public async Task HandleAsyncCreatesPronunciationWhenInputIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var words = new FakeWordRepository();
        var word = CreateWord("hello", now);
        words.Words.Add(word);
        var media = new FakeMediaRepository();
        var audio = CreateMedia("hello.mp3", MediaType.Audio, "audio/mpeg", now);
        var image = CreateMedia("mouth.jpg", MediaType.Image, "image/jpeg", now);
        media.Media.AddRange([audio, image]);
        var repository = new FakePronunciationRepository();
        var handler = new CreatePronunciationCommandHandler(
            repository,
            words,
            media,
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreatePronunciationCommand(
                word.Id,
                "/hallo/",
                "/hello/",
                "heh-lo",
                "hel-lo",
                "first",
                "mouth",
                "tongue",
                "mistake",
                "note",
                audio.Id,
                audio.Id,
                image.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Pronunciations);
        Assert.Equal(word.Id, result.Value!.WordId);
        Assert.Equal("hello", result.Value.Word!.Text);
        Assert.Equal(audio.Id, result.Value.AudioSlowMediaId);
        Assert.Equal(image.Id, result.Value.MouthImageMediaId);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenWordAlreadyHasPronunciation()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var words = new FakeWordRepository();
        var word = CreateWord("hello", now);
        words.Words.Add(word);
        var repository = new FakePronunciationRepository();
        repository.Pronunciations.Add(CreatePronunciation(word.Id, now));
        var handler = new CreatePronunciationCommandHandler(
            repository,
            words,
            new FakeMediaRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreatePronunciationCommand(word.Id, "/hallo/", null, null, null, null, null, null, null, null),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == nameof(CreatePronunciationCommand.WordId));
        Assert.Single(repository.Pronunciations);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenAudioSlotUsesImage()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var words = new FakeWordRepository();
        var word = CreateWord("hello", now);
        words.Words.Add(word);
        var media = new FakeMediaRepository();
        var image = CreateMedia("mouth.jpg", MediaType.Image, "image/jpeg", now);
        media.Media.Add(image);
        var repository = new FakePronunciationRepository();
        var handler = new CreatePronunciationCommandHandler(
            repository,
            words,
            media,
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreatePronunciationCommand(
                word.Id,
                "/hallo/",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                AudioSlowMediaId: image.Id),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "AudioSlowMediaId");
        Assert.Empty(repository.Pronunciations);
    }

    internal static Word CreateWord(string text, DateTimeOffset now)
    {
        return Word.Create(
            text,
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            text,
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);
    }

    internal static EnglishMaster.Domain.Pronunciations.Pronunciation CreatePronunciation(
        Guid wordId,
        DateTimeOffset now)
    {
        return EnglishMaster.Domain.Pronunciations.Pronunciation.Create(
            wordId,
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
            now);
    }

    internal static MediaEntity CreateMedia(
        string fileName,
        MediaType mediaType,
        string contentType,
        DateTimeOffset now)
    {
        var extension = Path.GetExtension(fileName);
        return MediaEntity.Create(
            fileName,
            fileName,
            extension,
            contentType,
            128,
            mediaType,
            $"media/{fileName}",
            $"/media/{fileName}",
            string.Empty,
            string.Empty,
            now);
    }
}
