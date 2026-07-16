namespace EnglishMaster.Contracts.Courses;

public sealed record ReorderCourseLessonsRequest(
    IReadOnlyList<Guid> OrderedCourseLessonIds);
