using EnglishMaster.Application.Features.Courses.Dtos;
using EnglishMaster.Domain.Courses;

namespace EnglishMaster.Application.Features.Courses;

public interface ICourseRepository
{
    Task AddAsync(Course course, CancellationToken cancellationToken);

    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedCourseId,
        CancellationToken cancellationToken);

    Task<CourseSearchResult> SearchAsync(
        CourseSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
