using EnglishMaster.Application.Features.Books.Dtos;
using EnglishMaster.Domain.Books;

namespace EnglishMaster.Application.Features.Books;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken cancellationToken);

    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedBookId,
        CancellationToken cancellationToken);

    Task<BookSearchResult> SearchAsync(
        BookSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
