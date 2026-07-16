using EnglishMaster.Contracts.Courses;

namespace EnglishMaster.Web.Services.Courses;

public interface ICourseApiClient
{
    Task<CourseSearchResponse> SearchAsync(CourseSearchRequest request, CancellationToken cancellationToken);

    Task<CourseDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<CourseDto> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken);

    Task<CourseDto> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<CourseDto> PublishAsync(Guid id, CancellationToken cancellationToken);

    Task<CourseDto> UnpublishAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CourseLessonDto>> GetLessonsAsync(Guid courseId, CancellationToken cancellationToken);

    Task<CourseDto> AddLessonAsync(
        Guid courseId,
        Guid lessonId,
        int sortOrder,
        bool isRequired,
        CancellationToken cancellationToken);

    Task RemoveLessonAsync(Guid courseId, Guid lessonId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CourseLessonDto>> ReorderLessonsAsync(
        Guid courseId,
        IReadOnlyList<Guid> orderedCourseLessonIds,
        CancellationToken cancellationToken);
}
