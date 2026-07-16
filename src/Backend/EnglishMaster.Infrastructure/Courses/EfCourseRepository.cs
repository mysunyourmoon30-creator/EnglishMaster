using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Courses;

internal sealed class EfCourseRepository : ICourseRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfCourseRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken)
    {
        await dbContext.Courses.AddAsync(course, cancellationToken);
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Courses
            .Include(course => course.Lessons)
            .FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedCourseId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Courses.AsNoTracking()
            .Where(course => course.Slug == slug);

        if (excludedCourseId.HasValue)
        {
            query = query.Where(course => course.Id != excludedCourseId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<CourseSearchResult> SearchAsync(
        CourseSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Course> query = dbContext.Courses.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(course => course.IsActive == criteria.IsActive.Value);
        }

        if (criteria.IsPublished.HasValue)
        {
            query = query.Where(course => course.IsPublished == criteria.IsPublished.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(course => course.CefrLevel == criteria.CefrLevel.Value);
        }

        if (criteria.CategoryId.HasValue)
        {
            query = query.Where(course => course.CategoryId == criteria.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(course =>
                course.Title.ToLower().Contains(searchTerm) ||
                course.Slug.ToLower().Contains(searchTerm) ||
                course.Summary.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new CourseSearchResult([], totalCount);
        }

        var items = await ApplySorting(query, criteria)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new CourseSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Course> ApplySorting(
        IQueryable<Course> query,
        CourseSearchCriteria criteria)
    {
        return (criteria.SortBy, criteria.SortDirection) switch
        {
            (CourseSortBy.CreatedAt, CourseSortDirection.Desc) => query
                .OrderByDescending(course => course.CreatedAt)
                .ThenBy(course => course.Title),
            (CourseSortBy.CreatedAt, _) => query
                .OrderBy(course => course.CreatedAt)
                .ThenBy(course => course.Title),
            (CourseSortBy.Title, CourseSortDirection.Desc) => query
                .OrderByDescending(course => course.Title)
                .ThenBy(course => course.Id),
            _ => query
                .OrderBy(course => course.Title)
                .ThenBy(course => course.Id)
        };
    }
}
