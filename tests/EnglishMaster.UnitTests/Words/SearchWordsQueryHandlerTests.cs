using EnglishMaster.Application.Features.Words;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Application.Features.Words.Queries;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using EnglishMaster.UnitTests.TestDoubles;
using MediaEntity = EnglishMaster.Domain.Media.Media;
using MediaType = EnglishMaster.Domain.Media.MediaType;

namespace EnglishMaster.UnitTests.Words;

public sealed class SearchWordsQueryHandlerTests
{
    [Fact]
    public async Task HandleAsyncSearchesActiveWordsByDefault()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var activeMatch = Word.Create(
            "Hello World",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "greeting",
            PartOfSpeech.Interjection,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);
        var inactiveMatch = Word.Create(
            "Hello Again",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "greeting",
            PartOfSpeech.Interjection,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);
        inactiveMatch.Deactivate(now.AddMinutes(1));
        var activeNonMatch = Word.Create(
            "Goodbye",
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            "farewell",
            PartOfSpeech.Interjection,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);
        var repository = new FakeWordRepository([activeMatch, inactiveMatch, activeNonMatch]);
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(
            new SearchWordsQuery("hello", null, null, null),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(repository.LastCriteria?.IsActive);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(activeMatch.Id, item.Id);
        Assert.Equal("hello-world", item.Slug);
        Assert.Equal(1, result.Value.PageNumber);
        Assert.Equal(20, result.Value.PageSize);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task HandleAsyncReturnsValidationErrorWhenSearchIsTooLong()
    {
        var repository = new FakeWordRepository([]);
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(
            new SearchWordsQuery(new string('a', WordFieldLimits.Text + 1), null, null, null),
            CancellationToken.None);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains(result.Errors, error => error.Field == nameof(SearchWordsQuery.Search));
    }

    [Fact]
    public async Task HandleAsyncFiltersByCefrLevel()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var a1Word = CreateWord("cefr test alpha", "meaning", CefrLevel.A1, now);
        var b2Word = CreateWord("cefr test beta", "meaning", CefrLevel.B2, now);
        var repository = new FakeWordRepository([a1Word, b2Word]);
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(
            new SearchWordsQuery("cefr test", null, "B2", true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(b2Word.Id, item.Id);
    }

    [Fact]
    public async Task HandleAsyncFiltersByActiveStatus()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var activeWord = CreateWord("active status alpha", "meaning", CefrLevel.A1, now);
        var inactiveWord = CreateWord("active status beta", "meaning", CefrLevel.A1, now);
        inactiveWord.Deactivate(now.AddMinutes(1));
        var repository = new FakeWordRepository([activeWord, inactiveWord]);
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(
            new SearchWordsQuery("active status", null, null, false),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(inactiveWord.Id, item.Id);
    }

    [Fact]
    public async Task HandleAsyncPaginatesAndSortsByText()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var repository = new FakeWordRepository(
        [
            CreateWord("page test charlie", "meaning", CefrLevel.A1, now),
            CreateWord("page test alpha", "meaning", CefrLevel.A1, now),
            CreateWord("page test bravo", "meaning", CefrLevel.A1, now)
        ]);
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(
            new SearchWordsQuery("page test", null, null, true, 2, 2, "Text", "Asc"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal("page test charlie", item.Text);
        Assert.Equal(2, result.Value.PageNumber);
        Assert.Equal(2, result.Value.PageSize);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(2, result.Value.TotalPages);
        Assert.True(result.Value.HasPreviousPage);
        Assert.False(result.Value.HasNextPage);
    }

    [Fact]
    public async Task HandleAsyncFiltersByCategoryId()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var category = Category.Create("Basics", "Core words", 1, now);
        var matching = CreateWord("category filter alpha", "meaning", CefrLevel.A1, now);
        matching.SetCategory(category.Id, now);
        var nonMatching = CreateWord("category filter beta", "meaning", CefrLevel.A1, now);
        var categories = new FakeCategoryRepository();
        categories.Categories.Add(category);
        var repository = new FakeWordRepository([matching, nonMatching]);
        var handler = CreateHandler(repository, categories);

        var result = await handler.HandleAsync(
            new SearchWordsQuery("category filter", null, null, true, CategoryId: category.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(category.Id, repository.LastCriteria?.CategoryId);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(matching.Id, item.Id);
        Assert.Equal(category.Id, item.CategoryId);
        Assert.Equal("Basics", item.Category!.Name);
    }

    [Fact]
    public async Task HandleAsyncFiltersByTagId()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var tag = Tag.Create("Travel", "Travel words", now);
        var matching = CreateWord("tag filter alpha", "meaning", CefrLevel.A1, now);
        matching.SetTags([tag.Id], now);
        var nonMatching = CreateWord("tag filter beta", "meaning", CefrLevel.A1, now);
        var tags = new FakeTagRepository();
        tags.Tags.Add(tag);
        var repository = new FakeWordRepository([matching, nonMatching]);
        var handler = CreateHandler(repository, tags: tags);

        var result = await handler.HandleAsync(
            new SearchWordsQuery("tag filter", null, null, true, TagId: tag.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(tag.Id, repository.LastCriteria?.TagId);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(matching.Id, item.Id);
        var resultTag = Assert.Single(item.Tags);
        Assert.Equal(tag.Id, resultTag.Id);
        Assert.Equal("Travel", resultTag.Name);
    }

    [Fact]
    public async Task HandleAsyncMapsWordMedia()
    {
        var now = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var word = CreateWord("media map alpha", "meaning", CefrLevel.A1, now);
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
            string.Empty,
            now);
        word.SetImageMedia(image.Id, now);
        word.SetAudioMedia(audio.Id, now);
        var media = new FakeMediaRepository();
        media.Media.AddRange([image, audio]);
        var repository = new FakeWordRepository([word]);
        var handler = CreateHandler(repository, media: media);

        var result = await handler.HandleAsync(
            new SearchWordsQuery("media map", null, null, true),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!.Items);
        Assert.Equal(image.Id, item.ImageMediaId);
        Assert.Equal(audio.Id, item.AudioMediaId);
        Assert.Equal("Image", item.ImageMedia!.MediaType);
        Assert.Equal("Audio", item.AudioMedia!.MediaType);
    }

    private static Word CreateWord(
        string text,
        string meaningEn,
        CefrLevel cefrLevel,
        DateTimeOffset now)
    {
        return Word.Create(
            text,
            string.Empty,
            string.Empty,
            string.Empty,
            "Thai meaning",
            meaningEn,
            PartOfSpeech.Noun,
            cefrLevel,
            string.Empty,
            string.Empty,
            now);
    }

    private sealed class FakeWordRepository : IWordRepository
    {
        private readonly List<Word> words;

        public FakeWordRepository(IEnumerable<Word> words)
        {
            this.words = [.. words];
        }

        public WordSearchCriteria? LastCriteria { get; private set; }

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
            LastCriteria = criteria;

            var query = words.AsEnumerable();
            if (criteria.IsActive.HasValue)
            {
                query = query.Where(word => word.IsActive == criteria.IsActive.Value);
            }

            if (criteria.PartOfSpeech.HasValue)
            {
                query = query.Where(word => word.PartOfSpeech == criteria.PartOfSpeech.Value);
            }

            if (criteria.CefrLevel.HasValue)
            {
                query = query.Where(word => word.CefrLevel == criteria.CefrLevel.Value);
            }

            if (criteria.CategoryId.HasValue)
            {
                query = query.Where(word => word.CategoryId == criteria.CategoryId.Value);
            }

            if (criteria.TagId.HasValue)
            {
                query = query.Where(word => word.Tags.Any(tag => tag.TagId == criteria.TagId.Value));
            }

            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var term = criteria.SearchTerm.Trim();
                query = query.Where(word =>
                    word.Text.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    word.Slug.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    word.MeaningTh.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    word.MeaningEn.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            query = (criteria.SortBy, criteria.SortDirection) switch
            {
                (WordSortBy.CreatedAt, WordSortDirection.Desc) => query
                    .OrderByDescending(word => word.CreatedAt)
                    .ThenBy(word => word.Text),
                (WordSortBy.CreatedAt, _) => query
                    .OrderBy(word => word.CreatedAt)
                    .ThenBy(word => word.Text),
                (WordSortBy.Text, WordSortDirection.Desc) => query
                    .OrderByDescending(word => word.Text)
                    .ThenBy(word => word.Id),
                _ => query
                    .OrderBy(word => word.Text)
                    .ThenBy(word => word.Id)
            };

            var filtered = query.ToArray();
            var items = filtered
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToArray();

            return Task.FromResult(new WordSearchResult(items, filtered.Length));
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }

    private static SearchWordsQueryHandler CreateHandler(
        FakeWordRepository repository,
        FakeCategoryRepository? categories = null,
        FakeTagRepository? tags = null,
        FakeMediaRepository? media = null)
    {
        return new SearchWordsQueryHandler(
            repository,
            categories ?? new FakeCategoryRepository(),
            tags ?? new FakeTagRepository(),
            media ?? new FakeMediaRepository());
    }
}
