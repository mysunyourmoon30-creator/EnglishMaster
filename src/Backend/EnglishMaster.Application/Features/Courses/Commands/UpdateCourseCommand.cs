namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed record UpdateCourseCommand(
    Guid Id,
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
