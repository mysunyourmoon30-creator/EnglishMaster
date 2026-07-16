using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Lessons;

internal sealed class EfLessonRepository : ILessonRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfLessonRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        await dbContext.Lessons.AddAsync(lesson, cancellationToken);
    }

    public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Lessons
            .Include(lesson => lesson.Words)
            .Include(lesson => lesson.GrammarRules)
            .FirstOrDefaultAsync(lesson => lesson.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Lesson>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToArray();
        if (normalizedIds.Length == 0)
        {
            return [];
        }

        return await dbContext.Lessons.AsNoTracking()
            .Where(lesson => normalizedIds.Contains(lesson.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedLessonId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Lessons.AsNoTracking()
            .Where(lesson => lesson.Slug == slug);

        if (excludedLessonId.HasValue)
        {
            query = query.Where(lesson => lesson.Id != excludedLessonId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<LessonSearchResult> SearchAsync(
        LessonSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Lesson> query = dbContext.Lessons.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(lesson => lesson.IsActive == criteria.IsActive.Value);
        }

        if (criteria.IsPublished.HasValue)
        {
            query = query.Where(lesson => lesson.IsPublished == criteria.IsPublished.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(lesson => lesson.CefrLevel == criteria.CefrLevel.Value);
        }

        if (criteria.CategoryId.HasValue)
        {
            query = query.Where(lesson => lesson.CategoryId == criteria.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(lesson =>
                lesson.Title.ToLower().Contains(searchTerm) ||
                lesson.Slug.ToLower().Contains(searchTerm) ||
                lesson.Summary.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new LessonSearchResult([], totalCount);
        }

        var sortedQuery = ApplySorting(query, criteria);
        var items = await sortedQuery
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new LessonSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Lesson> ApplySorting(
        IQueryable<Lesson> query,
        LessonSearchCriteria criteria)
    {
        return (criteria.SortBy, criteria.SortDirection) switch
        {
            (LessonSortBy.CreatedAt, LessonSortDirection.Desc) => query
                .OrderByDescending(lesson => lesson.CreatedAt)
                .ThenBy(lesson => lesson.Title),
            (LessonSortBy.CreatedAt, _) => query
                .OrderBy(lesson => lesson.CreatedAt)
                .ThenBy(lesson => lesson.Title),
            (LessonSortBy.Title, LessonSortDirection.Desc) => query
                .OrderByDescending(lesson => lesson.Title)
                .ThenBy(lesson => lesson.Id),
            _ => query
                .OrderBy(lesson => lesson.Title)
                .ThenBy(lesson => lesson.Id)
        };
    }
}
