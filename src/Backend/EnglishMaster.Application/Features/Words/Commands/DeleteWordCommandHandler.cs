using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Words.Commands;

public sealed class DeleteWordCommandHandler
{
    private readonly IWordRepository wordRepository;
    private readonly TimeProvider timeProvider;

    public DeleteWordCommandHandler(IWordRepository wordRepository, TimeProvider timeProvider)
    {
        this.wordRepository = wordRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(DeleteWordCommand command, CancellationToken cancellationToken)
    {
        var word = await wordRepository.GetByIdAsync(command.Id, cancellationToken);
        if (word is null)
        {
            return Result.NotFound(nameof(command.Id), "Word was not found.");
        }

        word.Deactivate(timeProvider.GetUtcNow());
        await wordRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
