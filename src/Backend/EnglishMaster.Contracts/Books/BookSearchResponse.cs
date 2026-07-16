namespace EnglishMaster.Contracts.Books;

public sealed record BookSearchResponse(
    IReadOnlyCollection<BookDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
