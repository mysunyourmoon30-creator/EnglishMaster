using EnglishMaster.Application.Features.BookChapters;
using EnglishMaster.Domain.Books;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Books;

internal sealed class EfBookChapterRepository : IBookChapterRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfBookChapterRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(BookChapter chapter, CancellationToken cancellationToken)
    {
        await dbContext.BookChapters.AddAsync(chapter, cancellationToken);
    }

    public async Task<BookChapter?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.BookChapters
            .Include(chapter => chapter.Lessons)
            .FirstOrDefaultAsync(chapter => chapter.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BookChapter>> GetByBookIdAsync(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        return await dbContext.BookChapters
            .Include(chapter => chapter.Lessons)
            .Where(chapter => chapter.BookId == bookId)
            .OrderBy(chapter => chapter.SortOrder)
            .ThenBy(chapter => chapter.Title)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
