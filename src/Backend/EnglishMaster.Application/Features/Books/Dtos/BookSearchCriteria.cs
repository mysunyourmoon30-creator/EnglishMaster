using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Books.Dtos;

public sealed record BookSearchCriteria(
    string? SearchTerm,
    CefrLevel? CefrLevel,
    Guid? CategoryId,
    Guid? CourseId,
    bool? IsPublished,
    bool? IsActive,
    int PageNumber,
    int PageSize,
    BookSortBy SortBy,
    BookSortDirection SortDirection);

public sealed record BookSearchResult(
    IReadOnlyCollection<Book> Items,
    int TotalCount);

public enum BookSortBy
{
    Title = 0,
    CreatedAt = 1
}

public enum BookSortDirection
{
    Asc = 0,
    Desc = 1
}
