namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed record ReorderCourseLessonsCommand(
    Guid CourseId,
    IReadOnlyList<Guid> OrderedCourseLessonIds);
