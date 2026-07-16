using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Books.Dtos;
using EnglishMaster.Domain.Books;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Books;

internal sealed class EfBookRepository : IBookRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfBookRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken)
    {
        await dbContext.Books.AddAsync(book, cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Books
            .Include(book => book.Chapters)
            .ThenInclude(chapter => chapter.Lessons)
            .FirstOrDefaultAsync(book => book.Id == id, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedBookId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Books.AsNoTracking()
            .Where(book => book.Slug == slug);

        if (excludedBookId.HasValue)
        {
            query = query.Where(book => book.Id != excludedBookId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<BookSearchResult> SearchAsync(
        BookSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Book> query = dbContext.Books.AsNoTracking();

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
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(book =>
                book.Title.ToLower().Contains(searchTerm) ||
                book.Slug.ToLower().Contains(searchTerm) ||
                book.Summary.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new BookSearchResult([], totalCount);
        }

        var items = await ApplySorting(query, criteria)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new BookSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Book> ApplySorting(
        IQueryable<Book> query,
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
