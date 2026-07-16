using EnglishMaster.Application.Features.LessonSections;
using EnglishMaster.Domain.Lessons;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeLessonSectionRepository : ILessonSectionRepository
{
    public List<LessonSection> LessonSections { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(LessonSection lessonSection, CancellationToken cancellationToken)
    {
        LessonSections.Add(lessonSection);
        return Task.CompletedTask;
    }

    public Task<LessonSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(LessonSections.SingleOrDefault(section => section.Id == id));
    }

    public Task<IReadOnlyCollection<LessonSection>> GetByLessonIdAsync(
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<LessonSection> sections = LessonSections
            .Where(section => section.LessonId == lessonId)
            .OrderBy(section => section.SortOrder)
            .ThenBy(section => section.Title)
            .ToArray();

        return Task.FromResult(sections);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
