using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Courses.Queries;

public sealed class SearchCoursesQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = CourseFieldLimits.Title;

    private readonly ICourseRepository courseRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ILessonRepository lessonRepository;

    public SearchCoursesQueryHandler(
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ILessonRepository lessonRepository)
    {
        this.courseRepository = courseRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.lessonRepository = lessonRepository;
    }

    public async Task<Result<CourseSearchResponse>> HandleAsync(
        SearchCoursesQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        var cefrLevel = ParseOptionalEnum<CefrLevel>(query.CefrLevel, nameof(query.CefrLevel), errors);
        ValidateOptionalId(query.CategoryId, nameof(query.CategoryId), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);
        var sortBy = ParseOptionalEnum(query.SortBy, nameof(query.SortBy), CourseSortBy.Title, errors);
        var sortDirection = ParseOptionalEnum(
            query.SortDirection,
            nameof(query.SortDirection),
            CourseSortDirection.Asc,
            errors);

        if (errors.Count > 0)
        {
            return Result<CourseSearchResponse>.Validation([.. errors]);
        }

        var criteria = new CourseSearchCriteria(
            search,
            cefrLevel,
            query.CategoryId,
            query.IsPublished,
            query.IsActive ?? true,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection);

        var searchResult = await courseRepository.SearchAsync(criteria, cancellationToken);
        var items = await CourseReadModelBuilder.MapAsync(
            searchResult.Items,
            categoryRepository,
            mediaRepository,
            lessonRepository,
            cancellationToken);
        var totalPages = searchResult.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(searchResult.TotalCount / (double)pageSize);

        return Result<CourseSearchResponse>.Success(new CourseSearchResponse(
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
                nameof(SearchCoursesQuery.PageNumber),
                $"{nameof(SearchCoursesQuery.PageNumber)} must be greater than or equal to 1."));
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
                nameof(SearchCoursesQuery.PageSize),
                $"{nameof(SearchCoursesQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
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
                nameof(SearchCoursesQuery.Search),
                $"{nameof(SearchCoursesQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
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
