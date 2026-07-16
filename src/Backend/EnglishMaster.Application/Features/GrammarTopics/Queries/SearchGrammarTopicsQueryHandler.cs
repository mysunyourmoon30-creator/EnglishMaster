using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarTopics.Queries;

public sealed class SearchGrammarTopicsQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = GrammarTopicFieldLimits.Title;

    private readonly IGrammarTopicRepository grammarTopicRepository;

    public SearchGrammarTopicsQueryHandler(IGrammarTopicRepository grammarTopicRepository)
    {
        this.grammarTopicRepository = grammarTopicRepository;
    }

    public async Task<Result<GrammarTopicSearchResponse>> HandleAsync(
        SearchGrammarTopicsQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        var cefrLevel = ParseOptionalEnum<CefrLevel>(query.CefrLevel, nameof(query.CefrLevel), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);

        if (errors.Count > 0)
        {
            return Result<GrammarTopicSearchResponse>.Validation([.. errors]);
        }

        var result = await grammarTopicRepository.SearchAsync(
            new GrammarTopicSearchCriteria(search, cefrLevel, query.IsActive ?? true, pageNumber, pageSize),
            cancellationToken);
        var totalPages = result.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        return Result<GrammarTopicSearchResponse>.Success(new GrammarTopicSearchResponse(
            result.Items.Select(GrammarTopicMapper.ToDto).ToArray(),
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
            errors.Add(new ValidationError(nameof(SearchGrammarTopicsQuery.Search), $"{nameof(SearchGrammarTopicsQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
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
            errors.Add(new ValidationError(nameof(SearchGrammarTopicsQuery.PageNumber), $"{nameof(SearchGrammarTopicsQuery.PageNumber)} must be greater than or equal to 1."));
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
            errors.Add(new ValidationError(nameof(SearchGrammarTopicsQuery.PageSize), $"{nameof(SearchGrammarTopicsQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
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
