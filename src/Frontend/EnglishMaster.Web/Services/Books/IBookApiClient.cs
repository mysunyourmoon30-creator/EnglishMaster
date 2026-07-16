using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Contracts.Books;

namespace EnglishMaster.Web.Services.Books;

public interface IBookApiClient
{
    Task<BookSearchResponse> SearchAsync(BookSearchRequest request, CancellationToken cancellationToken);

    Task<BookDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken);

    Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<BookDto> PublishAsync(Guid id, CancellationToken cancellationToken);

    Task<BookDto> UnpublishAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BookChapterDto>> GetChaptersAsync(Guid bookId, CancellationToken cancellationToken);

    Task<BookChapterDto> AddChapterAsync(
        Guid bookId,
        CreateBookChapterRequest request,
        CancellationToken cancellationToken);

    Task<BookChapterDto> UpdateChapterAsync(
        Guid id,
        UpdateBookChapterRequest request,
        CancellationToken cancellationToken);

    Task DeleteChapterAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BookChapterDto>> ReorderChaptersAsync(
        Guid bookId,
        IReadOnlyList<Guid> orderedChapterIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BookChapterLessonDto>> GetChapterLessonsAsync(
        Guid chapterId,
        CancellationToken cancellationToken);

    Task<BookChapterDto> AddLessonAsync(
        Guid chapterId,
        Guid lessonId,
        int sortOrder,
        CancellationToken cancellationToken);

    Task RemoveLessonAsync(Guid chapterId, Guid lessonId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<BookChapterLessonDto>> ReorderChapterLessonsAsync(
        Guid chapterId,
        IReadOnlyList<Guid> orderedBookChapterLessonIds,
        CancellationToken cancellationToken);
}
