using EnglishMaster.Domain.Books;

namespace EnglishMaster.Application.Features.BookChapters;

public interface IBookChapterRepository
{
    Task AddAsync(BookChapter chapter, CancellationToken cancellationToken);

    Task<BookChapter?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BookChapter>> GetByBookIdAsync(
        Guid bookId,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
