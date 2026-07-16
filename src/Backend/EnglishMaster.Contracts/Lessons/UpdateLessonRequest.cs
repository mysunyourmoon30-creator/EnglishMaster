namespace EnglishMaster.Contracts.Lessons;

public sealed record UpdateLessonRequest(
    string Title,
    string? Summary,
    string? Description,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? ThumbnailMediaId,
    int EstimatedMinutes,
    int SortOrder,
    bool IsPublished,
    bool IsActive);
