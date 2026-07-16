using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Commands;

public sealed class ActivatePronunciationCommandHandler
{
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly TimeProvider timeProvider;

    public ActivatePronunciationCommandHandler(
        IPronunciationRepository pronunciationRepository,
        TimeProvider timeProvider)
    {
        this.pronunciationRepository = pronunciationRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PronunciationDto>> HandleAsync(
        ActivatePronunciationCommand command,
        CancellationToken cancellationToken)
    {
        var pronunciation = await pronunciationRepository.GetByIdAsync(command.Id, cancellationToken);
        if (pronunciation is null)
        {
            return Result<PronunciationDto>.NotFound(nameof(command.Id), "Pronunciation was not found.");
        }

        pronunciation.Activate(timeProvider.GetUtcNow());
        await pronunciationRepository.SaveChangesAsync(cancellationToken);

        return Result<PronunciationDto>.Success(PronunciationMapper.ToDto(pronunciation));
    }
}
