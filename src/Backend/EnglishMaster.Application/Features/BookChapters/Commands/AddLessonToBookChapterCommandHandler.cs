using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed class AddLessonToBookChapterCommandHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public AddLessonToBookChapterCommandHandler(
        IBookChapterRepository chapterRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.chapterRepository = chapterRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<BookChapterDto>> HandleAsync(
        AddLessonToBookChapterCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BookChapterId == Guid.Empty)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.BookChapterId), $"{nameof(command.BookChapterId)} cannot be empty."));
        }

        if (command.LessonId == Guid.Empty)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.LessonId), $"{nameof(command.LessonId)} cannot be empty."));
        }

        if (command.SortOrder < 0)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.SortOrder), $"{nameof(command.SortOrder)} must be greater than or equal to zero."));
        }

        var chapter = await chapterRepository.GetByIdAsync(command.BookChapterId, cancellationToken);
        if (chapter is null)
        {
            return Result<BookChapterDto>.NotFound(nameof(command.BookChapterId), "Book chapter was not found.");
        }

        if (!chapter.IsActive)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.BookChapterId), "Book chapter is inactive."));
        }

        var lesson = await lessonRepository.GetByIdAsync(command.LessonId, cancellationToken);
        if (lesson is null)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.LessonId), "Lesson was not found."));
        }

        if (!lesson.IsActive)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(command.LessonId), "Lesson is inactive."));
        }

        chapter.AddLesson(command.LessonId, command.SortOrder, timeProvider.GetUtcNow());
        await chapterRepository.SaveChangesAsync(cancellationToken);

        return Result<BookChapterDto>.Success(await BookChapterReadModelBuilder.MapAsync(
            chapter,
            lessonRepository,
            cancellationToken));
    }
}
