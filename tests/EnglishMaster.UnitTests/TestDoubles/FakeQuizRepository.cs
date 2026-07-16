using EnglishMaster.Application.Features.Quizzes;
using EnglishMaster.Application.Features.Quizzes.Dtos;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeQuizRepository : IQuizRepository
{
    public List<Quiz> Quizzes { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        Quizzes.Add(quiz);
        return Task.CompletedTask;
    }

    public Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Quizzes.SingleOrDefault(quiz => quiz.Id == id));
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedQuizId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Quizzes.Any(quiz =>
            quiz.Slug == slug &&
            (!excludedQuizId.HasValue || quiz.Id != excludedQuizId.Value)));
    }

    public Task<QuizSearchResult> SearchAsync(
        QuizSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Quizzes.AsEnumerable();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(quiz => quiz.IsActive == criteria.IsActive.Value);
        }

        if (criteria.IsPublished.HasValue)
        {
            query = query.Where(quiz => quiz.IsPublished == criteria.IsPublished.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(quiz => quiz.CefrLevel == criteria.CefrLevel.Value);
        }

        if (criteria.CategoryId.HasValue)
        {
            query = query.Where(quiz => quiz.CategoryId == criteria.CategoryId.Value);
        }

        if (criteria.LessonId.HasValue)
        {
            query = query.Where(quiz => quiz.LessonId == criteria.LessonId.Value);
        }

        if (criteria.CourseId.HasValue)
        {
            query = query.Where(quiz => quiz.CourseId == criteria.CourseId.Value);
        }

        if (criteria.BookId.HasValue)
        {
            query = query.Where(quiz => quiz.BookId == criteria.BookId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(quiz =>
                quiz.Title.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                quiz.Slug.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                quiz.Summary.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = ApplySorting(query, criteria).ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new QuizSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }

    private static IEnumerable<Quiz> ApplySorting(
        IEnumerable<Quiz> query,
        QuizSearchCriteria criteria)
    {
        return (criteria.SortBy, criteria.SortDirection) switch
        {
            (QuizSortBy.CreatedAt, QuizSortDirection.Desc) => query
                .OrderByDescending(quiz => quiz.CreatedAt)
                .ThenBy(quiz => quiz.Title),
            (QuizSortBy.CreatedAt, _) => query
                .OrderBy(quiz => quiz.CreatedAt)
                .ThenBy(quiz => quiz.Title),
            (QuizSortBy.Title, QuizSortDirection.Desc) => query
                .OrderByDescending(quiz => quiz.Title)
                .ThenBy(quiz => quiz.Id),
            _ => query
                .OrderBy(quiz => quiz.Title)
                .ThenBy(quiz => quiz.Id)
        };
    }
}
