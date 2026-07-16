using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Books.Dtos;
using EnglishMaster.Domain.Books;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeBookRepository : IBookRepository
{
    public List<Book> Books { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(Book book, CancellationToken cancellationToken)
    {
        Books.Add(book);
        return Task.CompletedTask;
    }

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Books.SingleOrDefault(book => book.Id == id));
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedBookId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Books.Any(book =>
            book.Slug == slug &&
            (!excludedBookId.HasValue || book.Id != excludedBookId.Value)));
    }

    public Task<BookSearchResult> SearchAsync(
        BookSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Books.AsEnumerable();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(book => book.IsActive == criteria.IsActive.Value);
        }

        if (criteria.IsPublished.HasValue)
        {
            query = query.Where(book => book.IsPublished == criteria.IsPublished.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(book => book.CefrLevel == criteria.CefrLevel.Value);
        }

        if (criteria.CategoryId.HasValue)
        {
            query = query.Where(book => book.CategoryId == criteria.CategoryId.Value);
        }

        if (criteria.CourseId.HasValue)
        {
            query = query.Where(book => book.CourseId == criteria.CourseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(book =>
                book.Title.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                book.Slug.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                book.Summary.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = ApplySorting(query, criteria).ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new BookSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }

    private static IEnumerable<Book> ApplySorting(
        IEnumerable<Book> query,
        BookSearchCriteria criteria)
    {
        return (criteria.SortBy, criteria.SortDirection) switch
        {
            (BookSortBy.CreatedAt, BookSortDirection.Desc) => query
                .OrderByDescending(book => book.CreatedAt)
                .ThenBy(book => book.Title),
            (BookSortBy.CreatedAt, _) => query
                .OrderBy(book => book.CreatedAt)
                .ThenBy(book => book.Title),
            (BookSortBy.Title, BookSortDirection.Desc) => query
                .OrderByDescending(book => book.Title)
                .ThenBy(book => book.Id),
            _ => query
                .OrderBy(book => book.Title)
                .ThenBy(book => book.Id)
        };
    }
}
