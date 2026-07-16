using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Domain.Lessons;

namespace EnglishMaster.Application.Features.Lessons;

public interface ILessonRepository
{
    Task AddAsync(Lesson lesson, CancellationToken cancellationToken);

    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Lesson>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedLessonId,
        CancellationToken cancellationToken);

    Task<LessonSearchResult> SearchAsync(
        LessonSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
