using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Queries;

public sealed class SearchPronunciationsQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = PronunciationFieldLimits.PracticeNote;

    private readonly IPronunciationRepository pronunciationRepository;
    private readonly IWordRepository wordRepository;
    private readonly IMediaRepository mediaRepository;

    public SearchPronunciationsQueryHandler(
        IPronunciationRepository pronunciationRepository,
        IWordRepository wordRepository,
        IMediaRepository mediaRepository)
    {
        this.pronunciationRepository = pronunciationRepository;
        this.wordRepository = wordRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<PronunciationSearchResponse>> HandleAsync(
        SearchPronunciationsQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        ValidateOptionalId(query.WordId, nameof(query.WordId), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);

        if (errors.Count > 0)
        {
            return Result<PronunciationSearchResponse>.Validation([.. errors]);
        }

        var result = await pronunciationRepository.SearchAsync(
            new PronunciationSearchCriteria(
                search,
                query.WordId,
                query.IsActive ?? true,
                pageNumber,
                pageSize),
            cancellationToken);

        var items = new List<PronunciationDto>();
        foreach (var pronunciation in result.Items)
        {
            items.Add(await PronunciationReadModelBuilder.MapAsync(
                pronunciation,
                wordRepository,
                mediaRepository,
                includeMinimalPairs: false,
                cancellationToken));
        }

        var totalPages = result.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        return Result<PronunciationSearchResponse>.Success(new PronunciationSearchResponse(
            items,
            pageNumber,
            pageSize,
            result.TotalCount,
            totalPages,
            pageNumber > 1 && totalPages > 0,
            pageNumber < totalPages));
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
                nameof(SearchPronunciationsQuery.Search),
                $"{nameof(SearchPronunciationsQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
        }

        return normalized;
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
                nameof(SearchPronunciationsQuery.PageNumber),
                $"{nameof(SearchPronunciationsQuery.PageNumber)} must be greater than or equal to 1."));
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
                nameof(SearchPronunciationsQuery.PageSize),
                $"{nameof(SearchPronunciationsQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
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
