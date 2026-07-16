using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Commands;

public sealed class DeletePronunciationCommandHandler
{
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly TimeProvider timeProvider;

    public DeletePronunciationCommandHandler(
        IPronunciationRepository pronunciationRepository,
        TimeProvider timeProvider)
    {
        this.pronunciationRepository = pronunciationRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeletePronunciationCommand command,
        CancellationToken cancellationToken)
    {
        var pronunciation = await pronunciationRepository.GetByIdAsync(command.Id, cancellationToken);
        if (pronunciation is null)
        {
            return Result.NotFound(nameof(command.Id), "Pronunciation was not found.");
        }

        pronunciation.Deactivate(timeProvider.GetUtcNow());
        await pronunciationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
