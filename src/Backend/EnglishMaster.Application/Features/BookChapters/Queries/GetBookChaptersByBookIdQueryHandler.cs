using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Queries;

public sealed class GetBookChaptersByBookIdQueryHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly IBookRepository bookRepository;
    private readonly ILessonRepository lessonRepository;

    public GetBookChaptersByBookIdQueryHandler(
        IBookChapterRepository chapterRepository,
        IBookRepository bookRepository,
        ILessonRepository lessonRepository)
    {
        this.chapterRepository = chapterRepository;
        this.bookRepository = bookRepository;
        this.lessonRepository = lessonRepository;
    }

    public async Task<Result<IReadOnlyCollection<BookChapterDto>>> HandleAsync(
        GetBookChaptersByBookIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.BookId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.Validation(
                new ValidationError(nameof(query.BookId), $"{nameof(query.BookId)} cannot be empty."));
        }

        var book = await bookRepository.GetByIdAsync(query.BookId, cancellationToken);
        if (book is null)
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.NotFound(nameof(query.BookId), "Book was not found.");
        }

        var chapters = await chapterRepository.GetByBookIdAsync(query.BookId, cancellationToken);
        return Result<IReadOnlyCollection<BookChapterDto>>.Success(
            await BookChapterReadModelBuilder.MapAsync(
                chapters,
                lessonRepository,
                cancellationToken));
    }
}
