namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed record UpdateLessonCommand(
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
