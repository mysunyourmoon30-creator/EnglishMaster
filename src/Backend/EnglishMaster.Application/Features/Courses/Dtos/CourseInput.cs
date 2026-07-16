using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Courses.Dtos;

internal sealed record CourseInput(
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
