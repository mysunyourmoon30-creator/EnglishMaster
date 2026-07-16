namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed record AddLessonToCourseCommand(
    Guid CourseId,
    Guid LessonId,
    int SortOrder,
    bool IsRequired);
