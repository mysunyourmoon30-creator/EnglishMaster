namespace EnglishMaster.Contracts.StudentProgress;

public sealed record StudentProgressItemDto(
    string ContentType,
    Guid ContentId,
    string Slug,
    string Title,
    string Summary,
    string Url,
    int ProgressPercent,
    string Status,
    DateTimeOffset LastAccessedAt);

public sealed record StudentProgressSummaryDto(
    int TotalTrackedItems,
    int InProgressCount,
    int CompletedCount,
    IReadOnlyCollection<StudentProgressItemDto> Lessons,
    IReadOnlyCollection<StudentProgressItemDto> Courses,
    IReadOnlyCollection<StudentProgressItemDto> Books);
