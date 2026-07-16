using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Domain.Books;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed class AddBookChapterCommandHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly IBookRepository bookRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public AddBookChapterCommandHandler(
        IBookChapterRepository chapterRepository,
        IBookRepository bookRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.chapterRepository = chapterRepository;
        this.bookRepository = bookRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<BookChapterDto>> HandleAsync(
        AddBookChapterCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BookId == Guid.Empty)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.BookId), $"{nameof(command.BookId)} cannot be empty."));
        }

        var book = await bookRepository.GetByIdAsync(command.BookId, cancellationToken);
        if (book is null)
        {
            return Result<BookChapterDto>.NotFound(nameof(command.BookId), "Book was not found.");
        }

        if (!book.IsActive)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.BookId), "Book is inactive."));
        }

        var validation = BookChapterInputValidator.Validate(
            command.Title,
            command.Summary,
            command.ContentMarkdown,
            command.SortOrder,
            isActive: true);
        if (!validation.IsSuccess)
        {
            return Result<BookChapterDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var chapter = BookChapter.Create(
            command.BookId,
            input.Title,
            input.Summary,
            input.ContentMarkdown,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await chapterRepository.AddAsync(chapter, cancellationToken);
        await chapterRepository.SaveChangesAsync(cancellationToken);

        return Result<BookChapterDto>.Success(await BookChapterReadModelBuilder.MapAsync(
            chapter,
            lessonRepository,
            cancellationToken));
    }
}
