using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed class UpdateBookChapterCommandHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public UpdateBookChapterCommandHandler(
        IBookChapterRepository chapterRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.chapterRepository = chapterRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<BookChapterDto>> HandleAsync(
        UpdateBookChapterCommand command,
        CancellationToken cancellationToken)
    {
        var chapter = await chapterRepository.GetByIdAsync(command.Id, cancellationToken);
        if (chapter is null)
        {
            return Result<BookChapterDto>.NotFound(nameof(command.Id), "Book chapter was not found.");
        }

        var validation = BookChapterInputValidator.Validate(
            command.Title,
            command.Summary,
            command.ContentMarkdown,
            command.SortOrder,
            command.IsActive);
        if (!validation.IsSuccess)
        {
            return Result<BookChapterDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        chapter.Update(
            input.Title,
            input.Summary,
            input.ContentMarkdown,
            input.SortOrder,
            input.IsActive,
            timeProvider.GetUtcNow());

        await chapterRepository.SaveChangesAsync(cancellationToken);

        return Result<BookChapterDto>.Success(await BookChapterReadModelBuilder.MapAsync(
            chapter,
            lessonRepository,
            cancellationToken));
    }
}
