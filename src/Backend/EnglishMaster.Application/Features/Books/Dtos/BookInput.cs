using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Books.Dtos;

internal sealed record BookInput(
    string Title,
    string Slug,
    string Subtitle,
    string Summary,
    string Description,
    CefrLevel? CefrLevel,
    Guid? CategoryId,
    Guid? CoverMediaId,
    Guid? CourseId,
    string AuthorName,
    string Edition,
    string Version,
    int EstimatedPages,
    int SortOrder,
    bool IsPublished,
    bool IsActive);
