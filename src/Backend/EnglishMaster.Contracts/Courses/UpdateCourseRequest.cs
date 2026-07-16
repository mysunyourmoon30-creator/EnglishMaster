namespace EnglishMaster.Contracts.Courses;

public sealed record UpdateCourseRequest(
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
