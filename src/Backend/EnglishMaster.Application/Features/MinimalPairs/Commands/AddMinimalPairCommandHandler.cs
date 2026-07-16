using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.MinimalPairs.Dtos;
using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.MinimalPairs.Commands;

public sealed class AddMinimalPairCommandHandler
{
    private readonly IMinimalPairRepository minimalPairRepository;
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public AddMinimalPairCommandHandler(
        IMinimalPairRepository minimalPairRepository,
        IPronunciationRepository pronunciationRepository,
        IMediaRepository mediaRepository,
        TimeProvider timeProvider)
    {
        this.minimalPairRepository = minimalPairRepository;
        this.pronunciationRepository = pronunciationRepository;
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<MinimalPairDto>> HandleAsync(
        AddMinimalPairCommand command,
        CancellationToken cancellationToken)
    {
        if (command.PronunciationId == Guid.Empty)
        {
            return Result<MinimalPairDto>.Validation(
                new ValidationError(nameof(command.PronunciationId), $"{nameof(command.PronunciationId)} cannot be empty."));
        }

        var pronunciation = await pronunciationRepository.GetByIdAsync(command.PronunciationId, cancellationToken);
        if (pronunciation is null)
        {
            return Result<MinimalPairDto>.NotFound(nameof(command.PronunciationId), "Pronunciation was not found.");
        }

        if (!pronunciation.IsActive)
        {
            return Result<MinimalPairDto>.Validation(
                new ValidationError(nameof(command.PronunciationId), "Pronunciation is inactive."));
        }

        var validation = MinimalPairInputValidator.Validate(
            command.PairWordText,
            command.PairIpa,
            command.PairThaiReading,
            command.DifferenceNote,
            command.AudioMediaId,
            command.SortOrder,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<MinimalPairDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceValidation = await MinimalPairReferenceValidator.ValidateAsync(
            mediaRepository,
            input,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<MinimalPairDto>.Validation([.. referenceValidation.Errors]);
        }

        var minimalPair = MinimalPair.Create(
            command.PronunciationId,
            input.PairWordText,
            input.PairIpa,
            input.PairThaiReading,
            input.DifferenceNote,
            input.AudioMediaId,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await minimalPairRepository.AddAsync(minimalPair, cancellationToken);
        await minimalPairRepository.SaveChangesAsync(cancellationToken);

        return Result<MinimalPairDto>.Success(MinimalPairMapper.ToDto(
            minimalPair,
            referenceValidation.AudioMedia));
    }
}
