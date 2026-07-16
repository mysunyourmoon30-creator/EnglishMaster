using EnglishMaster.Domain.Lessons;

namespace EnglishMaster.Application.Features.LessonSections;

public interface ILessonSectionRepository
{
    Task AddAsync(LessonSection lessonSection, CancellationToken cancellationToken);

    Task<LessonSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LessonSection>> GetByLessonIdAsync(
        Guid lessonId,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
