using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Quizzes.Dtos;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Quizzes.Queries;

public sealed class SearchQuizzesQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;

    private readonly IQuizRepository quizRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly ICourseRepository courseRepository;
    private readonly IBookRepository bookRepository;

    public SearchQuizzesQueryHandler(IQuizRepository quizRepository, ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ICourseRepository courseRepository, IBookRepository bookRepository)
    {
        this.quizRepository = quizRepository;
        this.categoryRepository = categoryRepository;
        this.lessonRepository = lessonRepository;
        this.courseRepository = courseRepository;
        this.bookRepository = bookRepository;
    }

    public async Task<Result<QuizSearchResponse>> HandleAsync(SearchQuizzesQuery query, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        var cefrLevel = ParseOptionalEnum<CefrLevel>(query.CefrLevel, nameof(query.CefrLevel), errors);
        ValidateOptionalId(query.CategoryId, nameof(query.CategoryId), errors);
        ValidateOptionalId(query.LessonId, nameof(query.LessonId), errors);
        ValidateOptionalId(query.CourseId, nameof(query.CourseId), errors);
        ValidateOptionalId(query.BookId, nameof(query.BookId), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);
        var sortBy = ParseOptionalEnum(query.SortBy, nameof(query.SortBy), QuizSortBy.Title, errors);
        var sortDirection = ParseOptionalEnum(query.SortDirection, nameof(query.SortDirection), QuizSortDirection.Asc, errors);

        if (errors.Count > 0)
        {
            return Result<QuizSearchResponse>.Validation([.. errors]);
        }

        var searchResult = await quizRepository.SearchAsync(new QuizSearchCriteria(
            search,
            cefrLevel,
            query.CategoryId,
            query.LessonId,
            query.CourseId,
            query.BookId,
            query.IsPublished,
            query.IsActive ?? true,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection), cancellationToken);
        var items = await QuizReadModelBuilder.MapAsync(searchResult.Items, categoryRepository, lessonRepository, courseRepository, bookRepository, cancellationToken);
        var totalPages = searchResult.TotalCount == 0 ? 0 : (int)Math.Ceiling(searchResult.TotalCount / (double)pageSize);

        return Result<QuizSearchResponse>.Success(new QuizSearchResponse(items, pageNumber, pageSize, searchResult.TotalCount, totalPages, pageNumber > 1 && totalPages > 0, pageNumber < totalPages));
    }

    private static string? NormalizeSearch(string? value, ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > QuizFieldLimits.Title)
        {
            errors.Add(new ValidationError(nameof(SearchQuizzesQuery.Search), $"{nameof(SearchQuizzesQuery.Search)} must be {QuizFieldLimits.Title} characters or fewer."));
        }

        return normalized;
    }

    private static int NormalizePageNumber(int? value, ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageNumber;
        }

        if (value.Value < 1)
        {
            errors.Add(new ValidationError(nameof(SearchQuizzesQuery.PageNumber), $"{nameof(SearchQuizzesQuery.PageNumber)} must be greater than or equal to 1."));
            return DefaultPageNumber;
        }

        return value.Value;
    }

    private static int NormalizePageSize(int? value, ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageSize;
        }

        if (value.Value < 1 || value.Value > MaximumPageSize)
        {
            errors.Add(new ValidationError(nameof(SearchQuizzesQuery.PageSize), $"{nameof(SearchQuizzesQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
    }

    private static TEnum? ParseOptionalEnum<TEnum>(string? value, string field, ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return null;
    }

    private static TEnum ParseOptionalEnum<TEnum>(string? value, string field, TEnum defaultValue, ICollection<ValidationError> errors)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
        {
            return parsed;
        }

        errors.Add(new ValidationError(field, $"{field} is invalid."));
        return defaultValue;
    }

    private static void ValidateOptionalId(Guid? value, string field, ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
        }
    }
}
