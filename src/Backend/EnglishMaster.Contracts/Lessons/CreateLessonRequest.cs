namespace EnglishMaster.Contracts.Lessons;

public sealed record CreateLessonRequest(
    string Title,
    string? Summary,
    string? Description,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? ThumbnailMediaId,
    int EstimatedMinutes,
    int SortOrder);
