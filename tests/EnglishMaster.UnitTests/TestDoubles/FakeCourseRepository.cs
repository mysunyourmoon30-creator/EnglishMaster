using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Domain.Courses;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeCourseRepository : ICourseRepository
{
    public List<Course> Courses { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(Course course, CancellationToken cancellationToken)
    {
        Courses.Add(course);
        return Task.CompletedTask;
    }

    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Courses.SingleOrDefault(course => course.Id == id));
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedCourseId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Courses.Any(course =>
            course.Slug == slug &&
            (!excludedCourseId.HasValue || course.Id != excludedCourseId.Value)));
    }

    public Task<CourseSearchResult> SearchAsync(
        CourseSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Courses.AsEnumerable();

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
            query = query.Where(course =>
                course.Title.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                course.Slug.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                course.Summary.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = ApplySorting(query, criteria).ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new CourseSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }

    private static IEnumerable<Course> ApplySorting(
        IEnumerable<Course> query,
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
