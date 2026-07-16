using EnglishMaster.Application.Features.Words;
using EnglishMaster.Application.Features.Words.Commands;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;
using MediaEntity = EnglishMaster.Domain.Media.Media;
using MediaType = EnglishMaster.Domain.Media.MediaType;

namespace EnglishMaster.UnitTests.Words;

public sealed class CreateWordCommandHandlerTests
{
    [Fact]
    public async Task HandleAsyncCreatesWordWhenInputIsValid()
    {
        var repository = new FakeWordRepository();
        var categories = new FakeCategoryRepository();
        var tags = new FakeTagRepository();
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var handler = new CreateWordCommandHandler(
            repository,
            categories,
            tags,
            new FakeMediaRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateWordCommand(
                " hello ",
                "/he'lo/",
                "/he'lo/",
                "heh-lo",
                "สวัสดี",
                "greeting",
                "Noun",
                "A1",
                "Hello.",
                "สวัสดี"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(repository.Words);
        Assert.Equal("hello", result.Value!.Text);
        Assert.Equal("hello", result.Value.Slug);
        Assert.Equal(now, result.Value.CreatedAt);
    }

    [Fact]
    public async Task HandleAsyncCreatesWordWithCategoryAndTags()
    {
        var repository = new FakeWordRepository();
        var categories = new FakeCategoryRepository();
        var tags = new FakeTagRepository();
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var category = Category.Create("Basics", "Core words", 1, now);
        var tag = Tag.Create("Travel", "Travel words", now);
        categories.Categories.Add(category);
        tags.Tags.Add(tag);
        var handler = new CreateWordCommandHandler(
            repository,
            categories,
            tags,
            new FakeMediaRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateWordCommand(
                "airport",
                null,
                null,
                null,
                "Thai meaning",
                "airport",
                "Noun",
                "A1",
                null,
                null,
                category.Id,
                [tag.Id]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var word = Assert.Single(repository.Words);
        Assert.Equal(category.Id, word.CategoryId);
        Assert.Contains(word.Tags, wordTag => wordTag.TagId == tag.Id);
        Assert.Equal(category.Id, result.Value!.CategoryId);
        Assert.Equal("Basics", result.Value.Category!.Name);
        var resultTag = Assert.Single(result.Value.Tags);
        Assert.Equal(tag.Id, resultTag.Id);
    }

    [Fact]
    public async Task HandleAsyncCreatesWordWithImageAndAudioMedia()
    {
        var repository = new FakeWordRepository();
        var media = new FakeMediaRepository();
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var image = MediaEntity.Create(
            "image.jpg",
            "image.jpg",
            ".jpg",
            "image/jpeg",
            120,
            MediaType.Image,
            "media/image.jpg",
            "/media/image.jpg",
            "Image alt",
            "Image description",
            now);
        var audio = MediaEntity.Create(
            "audio.mp3",
            "audio.mp3",
            ".mp3",
            "audio/mpeg",
            240,
            MediaType.Audio,
            "media/audio.mp3",
            "/media/audio.mp3",
            "Audio alt",
            "Audio description",
            now);
        media.Media.AddRange([image, audio]);
        var handler = new CreateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            media,
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateWordCommand(
                "listen",
                null,
                null,
                null,
                "Thai meaning",
                "listen",
                "Verb",
                "A1",
                null,
                null,
                ImageMediaId: image.Id,
                AudioMediaId: audio.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var word = Assert.Single(repository.Words);
        Assert.Equal(image.Id, word.ImageMediaId);
        Assert.Equal(audio.Id, word.AudioMediaId);
        Assert.Equal(image.Id, result.Value!.ImageMediaId);
        Assert.Equal(audio.Id, result.Value.AudioMediaId);
        Assert.Equal("Image", result.Value.ImageMedia!.MediaType);
        Assert.Equal("Audio", result.Value.AudioMedia!.MediaType);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenMediaTypeDoesNotMatchWordSlot()
    {
        var repository = new FakeWordRepository();
        var media = new FakeMediaRepository();
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var audio = MediaEntity.Create(
            "audio.mp3",
            "audio.mp3",
            ".mp3",
            "audio/mpeg",
            240,
            MediaType.Audio,
            "media/audio.mp3",
            "/media/audio.mp3",
            string.Empty,
            string.Empty,
            now);
        media.Media.Add(audio);
        var handler = new CreateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            media,
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateWordCommand(
                "listen",
                null,
                null,
                null,
                "Thai meaning",
                "listen",
                "Verb",
                "A1",
                null,
                null,
                ImageMediaId: audio.Id),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "ImageMediaId");
        Assert.Empty(repository.Words);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorsWhenRequiredInputIsMissing()
    {
        var repository = new FakeWordRepository();
        var handler = new CreateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            new FakeMediaRepository(),
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(
            new CreateWordCommand(
                string.Empty,
                null,
                null,
                null,
                string.Empty,
                "meaning",
                "NotAPart",
                "A1",
                null,
                null),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Empty(repository.Words);
        Assert.Contains(result.Errors, error => error.Field == "text");
        Assert.Contains(result.Errors, error => error.Field == "meaningTh");
        Assert.Contains(result.Errors, error => error.Field == "partOfSpeech");
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenSlugAlreadyExists()
    {
        var repository = new FakeWordRepository();
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        repository.Words.Add(Word.Create(
            "hello",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            string.Empty,
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now));
        var handler = new CreateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            new FakeMediaRepository(),
            new FixedTimeProvider(now));

        var result = await handler.HandleAsync(
            new CreateWordCommand(
                "hello!",
                null,
                null,
                null,
                "Thai meaning",
                null,
                "Noun",
                "A1",
                null,
                null),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == nameof(CreateWordCommand.Text));
        Assert.Single(repository.Words);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenTextCannotProduceSlug()
    {
        var repository = new FakeWordRepository();
        var handler = new CreateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            new FakeMediaRepository(),
            new FixedTimeProvider(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync(
            new CreateWordCommand(
                "!!!",
                null,
                null,
                null,
                "Thai meaning",
                null,
                "Noun",
                "A1",
                null,
                null),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "text");
        Assert.Empty(repository.Words);
    }

    private sealed class FakeWordRepository : IWordRepository
    {
        public List<Word> Words { get; } = [];

        public Task AddAsync(Word word, CancellationToken cancellationToken)
        {
            Words.Add(word);
            return Task.CompletedTask;
        }

        public Task<Word?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Words.SingleOrDefault(word => word.Id == id));
        }

        public Task<IReadOnlyCollection<Word>> GetByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            var normalizedIds = ids.Distinct().ToHashSet();
            return Task.FromResult<IReadOnlyCollection<Word>>(
                Words.Where(word => normalizedIds.Contains(word.Id)).ToArray());
        }

        public Task<bool> SlugExistsAsync(
            string slug,
            Guid? excludedWordId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Words.Any(word =>
                word.Slug == slug &&
                (!excludedWordId.HasValue || word.Id != excludedWordId.Value)));
        }

        public Task<WordSearchResult> SearchAsync(
            WordSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new WordSearchResult(Words, Words.Count));
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset now;

        public FixedTimeProvider(DateTimeOffset now)
        {
            this.now = now;
        }

        public override DateTimeOffset GetUtcNow() => now;
    }
}
