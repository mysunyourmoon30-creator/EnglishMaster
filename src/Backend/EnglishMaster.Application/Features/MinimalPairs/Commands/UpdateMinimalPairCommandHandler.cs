using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.MinimalPairs.Dtos;
using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.MinimalPairs.Commands;

public sealed class UpdateMinimalPairCommandHandler
{
    private readonly IMinimalPairRepository minimalPairRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public UpdateMinimalPairCommandHandler(
        IMinimalPairRepository minimalPairRepository,
        IMediaRepository mediaRepository,
        TimeProvider timeProvider)
    {
        this.minimalPairRepository = minimalPairRepository;
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<MinimalPairDto>> HandleAsync(
        UpdateMinimalPairCommand command,
        CancellationToken cancellationToken)
    {
        var minimalPair = await minimalPairRepository.GetByIdAsync(command.Id, cancellationToken);
        if (minimalPair is null)
        {
            return Result<MinimalPairDto>.NotFound(nameof(command.Id), "Minimal pair was not found.");
        }

        var validation = MinimalPairInputValidator.Validate(
            command.PairWordText,
            command.PairIpa,
            command.PairThaiReading,
            command.DifferenceNote,
            command.AudioMediaId,
            command.SortOrder,
            command.IsActive);

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

        minimalPair.Update(
            input.PairWordText,
            input.PairIpa,
            input.PairThaiReading,
            input.DifferenceNote,
            input.AudioMediaId,
            input.SortOrder,
            input.IsActive,
            timeProvider.GetUtcNow());

        await minimalPairRepository.SaveChangesAsync(cancellationToken);

        return Result<MinimalPairDto>.Success(MinimalPairMapper.ToDto(
            minimalPair,
            referenceValidation.AudioMedia));
    }
}
