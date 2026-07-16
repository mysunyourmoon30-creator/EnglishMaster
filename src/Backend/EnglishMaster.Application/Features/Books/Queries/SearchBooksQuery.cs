namespace EnglishMaster.Application.Features.Books.Queries;

public sealed record SearchBooksQuery(
    string? Search = null,
    string? CefrLevel = null,
    Guid? CategoryId = null,
    Guid? CourseId = null,
    bool? IsPublished = null,
    bool? IsActive = true,
    int? PageNumber = 1,
    int? PageSize = 20,
    string? SortBy = "Title",
    string? SortDirection = "Asc");
