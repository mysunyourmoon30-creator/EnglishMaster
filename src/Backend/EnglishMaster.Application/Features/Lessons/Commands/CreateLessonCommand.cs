namespace EnglishMaster.Application.Features.Lessons.Commands;

public sealed record CreateLessonCommand(
    string Title,
    string? Summary,
    string? Description,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? ThumbnailMediaId,
    int EstimatedMinutes,
    int SortOrder);
