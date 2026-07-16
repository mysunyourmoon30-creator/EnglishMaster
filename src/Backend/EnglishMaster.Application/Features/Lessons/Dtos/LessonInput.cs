using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Lessons.Dtos;

internal sealed record LessonInput(
    string Title,
    string Slug,
    string Summary,
    string Description,
    CefrLevel? CefrLevel,
    Guid? CategoryId,
    Guid? ThumbnailMediaId,
    int EstimatedMinutes,
    int SortOrder,
    bool IsPublished,
    bool IsActive);
