using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Commands;

public sealed class DeleteBookChapterCommandHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly TimeProvider timeProvider;

    public DeleteBookChapterCommandHandler(
        IBookChapterRepository chapterRepository,
        TimeProvider timeProvider)
    {
        this.chapterRepository = chapterRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteBookChapterCommand command,
        CancellationToken cancellationToken)
    {
        var chapter = await chapterRepository.GetByIdAsync(command.Id, cancellationToken);
        if (chapter is null)
        {
            return Result.NotFound(nameof(command.Id), "Book chapter was not found.");
        }

        chapter.Deactivate(timeProvider.GetUtcNow());
        await chapterRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
