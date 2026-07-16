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

public sealed class UpdateWordCommandHandlerTests
{
    [Fact]
    public async Task HandleAsyncUpdatesWordWhenInputIsValid()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var updatedAt = createdAt.AddHours(2);
        var word = Word.Create(
            "hello",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "greeting",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            createdAt);
        var repository = new FakeWordRepository([word]);
        var handler = new UpdateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            new FakeMediaRepository(),
            new FixedTimeProvider(updatedAt));

        var result = await handler.HandleAsync(
            new UpdateWordCommand(
                word.Id,
                "Good Bye",
                "/good-bye/",
                "/good-bye/",
                "good-bye",
                "Updated Thai meaning",
                "farewell",
                "Interjection",
                "A2",
                "Good bye.",
                "Updated example",
                false),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Good Bye", word.Text);
        Assert.Equal("good-bye", word.Slug);
        Assert.Equal("farewell", word.MeaningEn);
        Assert.Equal(PartOfSpeech.Interjection, word.PartOfSpeech);
        Assert.Equal(CefrLevel.A2, word.CefrLevel);
        Assert.False(word.IsActive);
        Assert.Equal(updatedAt, word.UpdatedAt);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task HandleAsyncUpdatesWordCategoryAndTags()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var updatedAt = createdAt.AddHours(2);
        var word = Word.Create(
            "hello",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "greeting",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            createdAt);
        var category = Category.Create("Basics", "Core words", 1, createdAt);
        var tag = Tag.Create("Travel", "Travel words", createdAt);
        var categories = new FakeCategoryRepository();
        var tags = new FakeTagRepository();
        categories.Categories.Add(category);
        tags.Tags.Add(tag);
        var repository = new FakeWordRepository([word]);
        var handler = new UpdateWordCommandHandler(
            repository,
            categories,
            tags,
            new FakeMediaRepository(),
            new FixedTimeProvider(updatedAt));

        var result = await handler.HandleAsync(
            new UpdateWordCommand(
                word.Id,
                "hello",
                null,
                null,
                null,
                "Thai meaning",
                "greeting",
                "Noun",
                "A1",
                null,
                null,
                true,
                category.Id,
                [tag.Id]),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(category.Id, word.CategoryId);
        Assert.Contains(word.Tags, wordTag => wordTag.TagId == tag.Id);
        Assert.Equal(category.Id, result.Value!.CategoryId);
        var resultTag = Assert.Single(result.Value.Tags);
        Assert.Equal(tag.Id, resultTag.Id);
    }

    [Fact]
    public async Task HandleAsyncUpdatesWordImageAndAudioMedia()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var updatedAt = createdAt.AddHours(2);
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
            createdAt);
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
            string.Empty,
            createdAt);
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
            string.Empty,
            createdAt);
        var media = new FakeMediaRepository();
        media.Media.AddRange([image, audio]);
        var repository = new FakeWordRepository([word]);
        var handler = new UpdateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            media,
            new FixedTimeProvider(updatedAt));

        var result = await handler.HandleAsync(
            new UpdateWordCommand(
                word.Id,
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
                true,
                ImageMediaId: image.Id,
                AudioMediaId: audio.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(image.Id, word.ImageMediaId);
        Assert.Equal(audio.Id, word.AudioMediaId);
        Assert.Equal(image.Id, result.Value!.ImageMediaId);
        Assert.Equal(audio.Id, result.Value.AudioMediaId);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenRenamedSlugAlreadyExists()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var existing = Word.Create(
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
            now);
        var target = Word.Create(
            "goodbye",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            string.Empty,
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);
        var repository = new FakeWordRepository([existing, target]);
        var handler = new UpdateWordCommandHandler(
            repository,
            new FakeCategoryRepository(),
            new FakeTagRepository(),
            new FakeMediaRepository(),
            new FixedTimeProvider(now.AddHours(1)));

        var result = await handler.HandleAsync(
            new UpdateWordCommand(
                target.Id,
                "hello!",
                null,
                null,
                null,
                "Thai meaning",
                null,
                "Noun",
                "A1",
                null,
                null,
                true),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Equal("goodbye", target.Text);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    private sealed class FakeWordRepository : IWordRepository
    {
        private readonly List<Word> words;

        public FakeWordRepository(IEnumerable<Word> words)
        {
            this.words = [.. words];
        }

        public int SaveChangesCount { get; private set; }

        public Task AddAsync(Word word, CancellationToken cancellationToken)
        {
            words.Add(word);
            return Task.CompletedTask;
        }

        public Task<Word?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(words.SingleOrDefault(word => word.Id == id));
        }

        public Task<IReadOnlyCollection<Word>> GetByIdsAsync(
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            var normalizedIds = ids.Distinct().ToHashSet();
            return Task.FromResult<IReadOnlyCollection<Word>>(
                words.Where(word => normalizedIds.Contains(word.Id)).ToArray());
        }

        public Task<bool> SlugExistsAsync(
            string slug,
            Guid? excludedWordId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(words.Any(word =>
                word.Slug == slug &&
                (!excludedWordId.HasValue || word.Id != excludedWordId.Value)));
        }

        public Task<WordSearchResult> SearchAsync(
            WordSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new WordSearchResult(words, words.Count));
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCount++;
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
