using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Lessons.Queries;

public sealed class SearchLessonsQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = LessonFieldLimits.Title;

    private readonly ILessonRepository lessonRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;

    public SearchLessonsQueryHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository)
    {
        this.lessonRepository = lessonRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.wordRepository = wordRepository;
        this.grammarRuleRepository = grammarRuleRepository;
    }

    public async Task<Result<LessonSearchResponse>> HandleAsync(
        SearchLessonsQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        var cefrLevel = ParseOptionalEnum<CefrLevel>(query.CefrLevel, nameof(query.CefrLevel), errors);
        ValidateOptionalId(query.CategoryId, nameof(query.CategoryId), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);
        var sortBy = ParseOptionalEnum(query.SortBy, nameof(query.SortBy), LessonSortBy.Title, errors);
        var sortDirection = ParseOptionalEnum(
            query.SortDirection,
            nameof(query.SortDirection),
            LessonSortDirection.Asc,
            errors);

        if (errors.Count > 0)
        {
            return Result<LessonSearchResponse>.Validation([.. errors]);
        }

        var criteria = new LessonSearchCriteria(
            search,
            cefrLevel,
            query.CategoryId,
            query.IsPublished,
            query.IsActive ?? true,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection);

        var searchResult = await lessonRepository.SearchAsync(criteria, cancellationToken);
        var items = await LessonReadModelBuilder.MapAsync(
            searchResult.Items,
            categoryRepository,
            mediaRepository,
            wordRepository,
            grammarRuleRepository,
            cancellationToken);
        var totalPages = searchResult.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(searchResult.TotalCount / (double)pageSize);

        return Result<LessonSearchResponse>.Success(new LessonSearchResponse(
            items,
            pageNumber,
            pageSize,
            searchResult.TotalCount,
            totalPages,
            pageNumber > 1 && totalPages > 0,
            pageNumber < totalPages));
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
                nameof(SearchLessonsQuery.PageNumber),
                $"{nameof(SearchLessonsQuery.PageNumber)} must be greater than or equal to 1."));
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
                nameof(SearchLessonsQuery.PageSize),
                $"{nameof(SearchLessonsQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
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
                nameof(SearchLessonsQuery.Search),
                $"{nameof(SearchLessonsQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
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
