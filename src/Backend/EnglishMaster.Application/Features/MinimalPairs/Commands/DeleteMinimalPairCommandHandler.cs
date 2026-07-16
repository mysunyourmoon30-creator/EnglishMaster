using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.MinimalPairs.Commands;

public sealed class DeleteMinimalPairCommandHandler
{
    private readonly IMinimalPairRepository minimalPairRepository;
    private readonly TimeProvider timeProvider;

    public DeleteMinimalPairCommandHandler(
        IMinimalPairRepository minimalPairRepository,
        TimeProvider timeProvider)
    {
        this.minimalPairRepository = minimalPairRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteMinimalPairCommand command,
        CancellationToken cancellationToken)
    {
        var minimalPair = await minimalPairRepository.GetByIdAsync(command.Id, cancellationToken);
        if (minimalPair is null)
        {
            return Result.NotFound(nameof(command.Id), "Minimal pair was not found.");
        }

        minimalPair.Deactivate(timeProvider.GetUtcNow());
        await minimalPairRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
