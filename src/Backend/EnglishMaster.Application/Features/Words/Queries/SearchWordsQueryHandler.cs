using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Tags;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Contracts.Words;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Words.Queries;

public sealed class SearchWordsQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = WordFieldLimits.Text;

    private readonly IWordRepository wordRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ITagRepository tagRepository;
    private readonly IMediaRepository mediaRepository;

    public SearchWordsQueryHandler(
        IWordRepository wordRepository,
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IMediaRepository mediaRepository)
    {
        this.wordRepository = wordRepository;
        this.categoryRepository = categoryRepository;
        this.tagRepository = tagRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<WordSearchResponse>> HandleAsync(
        SearchWordsQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        var partOfSpeech = ParseOptionalEnum<PartOfSpeech>(query.PartOfSpeech, nameof(query.PartOfSpeech), errors);
        var cefrLevel = ParseOptionalEnum<CefrLevel>(query.CefrLevel, nameof(query.CefrLevel), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);
        var sortBy = ParseOptionalEnum(query.SortBy, nameof(query.SortBy), WordSortBy.Text, errors);
        var sortDirection = ParseOptionalEnum(
            query.SortDirection,
            nameof(query.SortDirection),
            WordSortDirection.Asc,
            errors);
        ValidateOptionalId(query.CategoryId, nameof(query.CategoryId), errors);
        ValidateOptionalId(query.TagId, nameof(query.TagId), errors);

        if (errors.Count > 0)
        {
            return Result<WordSearchResponse>.Validation([.. errors]);
        }

        var criteria = new WordSearchCriteria(
            search,
            partOfSpeech,
            cefrLevel,
            query.IsActive ?? true,
            query.CategoryId,
            query.TagId,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection);

        var searchResult = await wordRepository.SearchAsync(criteria, cancellationToken);
        var items = await MapWordsAsync(searchResult.Items, cancellationToken);
        var totalPages = searchResult.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(searchResult.TotalCount / (double)pageSize);
        var response = new WordSearchResponse(
            items,
            pageNumber,
            pageSize,
            searchResult.TotalCount,
            totalPages,
            pageNumber > 1 && totalPages > 0,
            pageNumber < totalPages);

        return Result<WordSearchResponse>.Success(response);
    }

    private async Task<IReadOnlyCollection<WordDto>> MapWordsAsync(
        IReadOnlyCollection<Word> words,
        CancellationToken cancellationToken)
    {
        var categoryIds = words
            .Select(word => word.CategoryId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();
        var tagIds = words
            .SelectMany(word => word.Tags.Select(tag => tag.TagId))
            .Distinct()
            .ToArray();
        var mediaIds = words
            .SelectMany(word => new[] { word.ImageMediaId, word.AudioMediaId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();

        var categories = categoryIds.Length == 0
            ? []
            : await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var tags = tagIds.Length == 0
            ? []
            : await tagRepository.GetByIdsAsync(tagIds, cancellationToken);
        var media = mediaIds.Length == 0
            ? []
            : await mediaRepository.GetByIdsAsync(mediaIds, cancellationToken);

        var categoryById = categories.ToDictionary(category => category.Id);
        var tagById = tags.ToDictionary(tag => tag.Id);
        var mediaById = media.ToDictionary(item => item.Id);

        return words
            .Select(word =>
            {
                var category = word.CategoryId.HasValue &&
                    categoryById.TryGetValue(word.CategoryId.Value, out var foundCategory)
                        ? foundCategory
                        : null;
                var wordTags = word.Tags
                    .Select(wordTag => tagById.TryGetValue(wordTag.TagId, out var tag) ? tag : null)
                    .Where(tag => tag is not null)
                    .Cast<Tag>()
                    .ToArray();
                MediaEntity? imageMedia = word.ImageMediaId.HasValue &&
                    mediaById.TryGetValue(word.ImageMediaId.Value, out var foundImageMedia)
                        ? foundImageMedia
                        : null;
                MediaEntity? audioMedia = word.AudioMediaId.HasValue &&
                    mediaById.TryGetValue(word.AudioMediaId.Value, out var foundAudioMedia)
                        ? foundAudioMedia
                        : null;

                return WordMapper.ToDto(word, category, wordTags, imageMedia, audioMedia);
            })
            .ToArray();
    }

    private static int NormalizePageNumber(
        int? value,
        ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageNumber;
        }

        if (value.Value < 1)
        {
            errors.Add(new ValidationError(
                nameof(SearchWordsQuery.PageNumber),
                $"{nameof(SearchWordsQuery.PageNumber)} must be greater than or equal to 1."));
            return DefaultPageNumber;
        }

        return value.Value;
    }

    private static int NormalizePageSize(
        int? value,
        ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageSize;
        }

        if (value.Value < 1 || value.Value > MaximumPageSize)
        {
            errors.Add(new ValidationError(
                nameof(SearchWordsQuery.PageSize),
                $"{nameof(SearchWordsQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
    }

    private static string? NormalizeSearch(
        string? value,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > MaximumSearchLength)
        {
            errors.Add(new ValidationError(
                nameof(SearchWordsQuery.Search),
                $"{nameof(SearchWordsQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
        }

        return normalized;
    }

    private static TEnum? ParseOptionalEnum<TEnum>(
        string? value,
        string field,
        ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed) &&
            Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return null;
    }

    private static TEnum ParseOptionalEnum<TEnum>(
        string? value,
        string field,
        TEnum defaultValue,
        ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed) &&
            Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return defaultValue;
    }

    private static void ValidateOptionalId(
        Guid? value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
        }
    }
}
