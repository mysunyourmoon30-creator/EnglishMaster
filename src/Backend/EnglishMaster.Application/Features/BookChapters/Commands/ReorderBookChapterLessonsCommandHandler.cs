using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed class ReorderBookChapterLessonsCommandHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public ReorderBookChapterLessonsCommandHandler(
        IBookChapterRepository chapterRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.chapterRepository = chapterRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyCollection<BookChapterLessonDto>>> HandleAsync(
        ReorderBookChapterLessonsCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BookChapterId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<BookChapterLessonDto>>.Validation(
                new ValidationError(nameof(command.BookChapterId), $"{nameof(command.BookChapterId)} cannot be empty."));
        }

        var orderValidation = ValidateOrder(command.OrderedBookChapterLessonIds);
        if (!orderValidation.IsSuccess)
        {
            return Result<IReadOnlyCollection<BookChapterLessonDto>>.Validation([.. orderValidation.Errors]);
        }

        var chapter = await chapterRepository.GetByIdAsync(command.BookChapterId, cancellationToken);
        if (chapter is null)
        {
            return Result<IReadOnlyCollection<BookChapterLessonDto>>.NotFound(
                nameof(command.BookChapterId),
                "Book chapter was not found.");
        }

        var existingIds = chapter.Lessons.Select(lesson => lesson.Id).Order().ToArray();
        var requestedIds = command.OrderedBookChapterLessonIds.Order().ToArray();
        if (!existingIds.SequenceEqual(requestedIds))
        {
            return Result<IReadOnlyCollection<BookChapterLessonDto>>.Validation(
                new ValidationError(nameof(command.OrderedBookChapterLessonIds), "Order must include each chapter lesson exactly once."));
        }

        var now = timeProvider.GetUtcNow();
        for (var index = 0; index < command.OrderedBookChapterLessonIds.Count; index++)
        {
            var relation = chapter.Lessons.Single(lesson => lesson.Id == command.OrderedBookChapterLessonIds[index]);
            relation.Reorder(index, now);
        }

        await chapterRepository.SaveChangesAsync(cancellationToken);

        return Result<IReadOnlyCollection<BookChapterLessonDto>>.Success(
            await BookChapterReadModelBuilder.MapLessonsAsync(
                chapter.Lessons,
                lessonRepository,
                cancellationToken));
    }

    private static Result ValidateOrder(IReadOnlyList<Guid> orderedIds)
    {
        if (orderedIds.Count == 0)
        {
            return Result.Validation(
                new ValidationError(nameof(orderedIds), "At least one chapter lesson id is required."));
        }

        if (orderedIds.Any(id => id == Guid.Empty))
        {
            return Result.Validation(
                new ValidationError(nameof(orderedIds), "Chapter lesson ids cannot be empty."));
        }

        if (orderedIds.Distinct().Count() != orderedIds.Count)
        {
            return Result.Validation(
                new ValidationError(nameof(orderedIds), "Chapter lesson ids cannot contain duplicates."));
        }

        return Result.Success();
    }
}
