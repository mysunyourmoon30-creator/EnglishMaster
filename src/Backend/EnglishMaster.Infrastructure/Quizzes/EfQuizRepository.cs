using EnglishMaster.Application.Features.Quizzes;
using EnglishMaster.Application.Features.Quizzes.Dtos;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Quizzes;

internal sealed class EfQuizRepository : IQuizRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfQuizRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        await dbContext.Quizzes.AddAsync(quiz, cancellationToken);
    }

    public async Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Quizzes
            .Include(quiz => quiz.Questions)
            .ThenInclude(question => question.Choices)
            .FirstOrDefaultAsync(quiz => quiz.Id == id, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedQuizId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Quizzes.AsNoTracking()
            .Where(quiz => quiz.Slug == slug);

        if (excludedQuizId.HasValue)
        {
            query = query.Where(quiz => quiz.Id != excludedQuizId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<QuizSearchResult> SearchAsync(
        QuizSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Quiz> query = dbContext.Quizzes.AsNoTracking();

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
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(quiz =>
                quiz.Title.ToLower().Contains(searchTerm) ||
                quiz.Slug.ToLower().Contains(searchTerm) ||
                quiz.Summary.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new QuizSearchResult([], totalCount);
        }

        var items = await ApplySorting(query, criteria)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new QuizSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Quiz> ApplySorting(
        IQueryable<Quiz> query,
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
