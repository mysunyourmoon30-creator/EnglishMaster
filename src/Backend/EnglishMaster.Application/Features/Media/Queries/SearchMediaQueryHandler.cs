using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Queries;

public sealed class SearchMediaQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = Domain.Media.MediaFieldLimits.OriginalFileName;

    private readonly IMediaRepository mediaRepository;

    public SearchMediaQueryHandler(IMediaRepository mediaRepository)
    {
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<MediaSearchResponse>> HandleAsync(
        SearchMediaQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        var mediaType = ParseOptionalEnum<Domain.Media.MediaType>(query.MediaType, nameof(query.MediaType), errors);
        var contentType = NormalizeContentType(query.ContentType, errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);

        if (errors.Count > 0)
        {
            return Result<MediaSearchResponse>.Validation([.. errors]);
        }

        var result = await mediaRepository.SearchAsync(
            new MediaSearchCriteria(
                search,
                mediaType,
                contentType,
                query.IsActive ?? true,
                pageNumber,
                pageSize),
            cancellationToken);

        var totalPages = result.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        return Result<MediaSearchResponse>.Success(new MediaSearchResponse(
            result.Items.Select(MediaMapper.ToDto).ToArray(),
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
            errors.Add(new ValidationError(nameof(SearchMediaQuery.Search), $"{nameof(SearchMediaQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
        }

        return normalized;
    }

    private static string? NormalizeContentType(
        string? value,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > Domain.Media.MediaFieldLimits.ContentType)
        {
            errors.Add(new ValidationError(nameof(SearchMediaQuery.ContentType), $"{nameof(SearchMediaQuery.ContentType)} must be {Domain.Media.MediaFieldLimits.ContentType} characters or fewer."));
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
            errors.Add(new ValidationError(nameof(SearchMediaQuery.PageNumber), $"{nameof(SearchMediaQuery.PageNumber)} must be greater than or equal to 1."));
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
            errors.Add(new ValidationError(nameof(SearchMediaQuery.PageSize), $"{nameof(SearchMediaQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
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
}
