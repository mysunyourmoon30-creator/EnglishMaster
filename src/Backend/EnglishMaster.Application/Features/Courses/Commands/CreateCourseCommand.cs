namespace EnglishMaster.Application.Features.Courses.Commands;

public sealed record CreateCourseCommand(
    string Title,
    string? Summary,
    string? Description,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? ThumbnailMediaId,
    int EstimatedMinutes,
    int SortOrder);
