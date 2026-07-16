using EnglishMaster.Application.Features.BookChapters;
using EnglishMaster.Domain.Books;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeBookChapterRepository : IBookChapterRepository
{
    public List<BookChapter> Chapters { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(BookChapter chapter, CancellationToken cancellationToken)
    {
        Chapters.Add(chapter);
        return Task.CompletedTask;
    }

    public Task<BookChapter?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Chapters.SingleOrDefault(chapter => chapter.Id == id));
    }

    public Task<IReadOnlyCollection<BookChapter>> GetByBookIdAsync(
        Guid bookId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<BookChapter> chapters = Chapters
            .Where(chapter => chapter.BookId == bookId)
            .OrderBy(chapter => chapter.SortOrder)
            .ThenBy(chapter => chapter.Title)
            .ToArray();

        return Task.FromResult(chapters);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
