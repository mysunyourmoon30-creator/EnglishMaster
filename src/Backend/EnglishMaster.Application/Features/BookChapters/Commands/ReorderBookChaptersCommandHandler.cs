using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed class ReorderBookChaptersCommandHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly IBookRepository bookRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public ReorderBookChaptersCommandHandler(
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

    public async Task<Result<IReadOnlyCollection<BookChapterDto>>> HandleAsync(
        ReorderBookChaptersCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BookId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.Validation(
                new ValidationError(nameof(command.BookId), $"{nameof(command.BookId)} cannot be empty."));
        }

        if (command.OrderedChapterIds is null)
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.Validation(
                new ValidationError(nameof(command.OrderedChapterIds), $"{nameof(command.OrderedChapterIds)} is required."));
        }

        if (command.OrderedChapterIds.Any(id => id == Guid.Empty))
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.Validation(
                new ValidationError(nameof(command.OrderedChapterIds), $"{nameof(command.OrderedChapterIds)} cannot contain empty chapter ids."));
        }

        if (command.OrderedChapterIds.Count != command.OrderedChapterIds.Distinct().Count())
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.Validation(
                new ValidationError(nameof(command.OrderedChapterIds), $"{nameof(command.OrderedChapterIds)} cannot contain duplicate chapter ids."));
        }

        var book = await bookRepository.GetByIdAsync(command.BookId, cancellationToken);
        if (book is null)
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.NotFound(nameof(command.BookId), "Book was not found.");
        }

        var chapters = await chapterRepository.GetByBookIdAsync(command.BookId, cancellationToken);
        var existingIds = chapters.Select(chapter => chapter.Id).ToHashSet();
        var providedIds = command.OrderedChapterIds.ToHashSet();

        if (chapters.Count != command.OrderedChapterIds.Count || !existingIds.SetEquals(providedIds))
        {
            return Result<IReadOnlyCollection<BookChapterDto>>.Validation(
                new ValidationError(
                    nameof(command.OrderedChapterIds),
                    "orderedChapterIds must contain exactly the current chapters of this book."));
        }

        var chapterById = chapters.ToDictionary(chapter => chapter.Id);
        var now = timeProvider.GetUtcNow();
        for (var index = 0; index < command.OrderedChapterIds.Count; index++)
        {
            chapterById[command.OrderedChapterIds[index]].Reorder(index, now);
        }

        await chapterRepository.SaveChangesAsync(cancellationToken);

        var ordered = command.OrderedChapterIds.Select(id => chapterById[id]).ToArray();
        var items = await BookChapterReadModelBuilder.MapAsync(ordered, lessonRepository, cancellationToken);

        return Result<IReadOnlyCollection<BookChapterDto>>.Success(items);
    }
}
