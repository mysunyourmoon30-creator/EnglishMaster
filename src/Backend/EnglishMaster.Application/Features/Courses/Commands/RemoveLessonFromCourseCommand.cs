namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed record RemoveLessonFromCourseCommand(
    Guid CourseId,
    Guid LessonId);
