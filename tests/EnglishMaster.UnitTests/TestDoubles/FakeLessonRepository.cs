using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Domain.Lessons;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeLessonRepository : ILessonRepository
{
    public List<Lesson> Lessons { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        Lessons.Add(lesson);
        return Task.CompletedTask;
    }

    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Lessons.SingleOrDefault(lesson => lesson.Id == id));
    }

    public Task<IReadOnlyCollection<Lesson>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.ToHashSet();
        IReadOnlyCollection<Lesson> lessons = Lessons
            .Where(lesson => normalizedIds.Contains(lesson.Id))
            .ToArray();

        return Task.FromResult(lessons);
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedLessonId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Lessons.Any(lesson =>
            lesson.Slug == slug &&
            (!excludedLessonId.HasValue || lesson.Id != excludedLessonId.Value)));
    }

    public Task<LessonSearchResult> SearchAsync(
        LessonSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Lessons.AsEnumerable();

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
            query = query.Where(lesson =>
                lesson.Title.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                lesson.Summary.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query
            .OrderBy(lesson => lesson.Title)
            .ThenBy(lesson => lesson.Id)
            .ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new LessonSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
