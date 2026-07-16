namespace EnglishMaster.Contracts.Books;

public sealed record BookSearchRequest(
    string? Search = null,
    string? CefrLevel = null,
    Guid? CategoryId = null,
    Guid? CourseId = null,
    bool? IsPublished = null,
    bool? IsActive = true,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "Title",
    string? SortDirection = "Asc");
