using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Commands;

public sealed class UpdatePronunciationCommandHandler
{
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly IWordRepository wordRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public UpdatePronunciationCommandHandler(
        IPronunciationRepository pronunciationRepository,
        IWordRepository wordRepository,
        IMediaRepository mediaRepository,
        TimeProvider timeProvider)
    {
        this.pronunciationRepository = pronunciationRepository;
        this.wordRepository = wordRepository;
        this.mediaRepository = mediaRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<PronunciationDto>> HandleAsync(
        UpdatePronunciationCommand command,
        CancellationToken cancellationToken)
    {
        var pronunciation = await pronunciationRepository.GetByIdAsync(command.Id, cancellationToken);
        if (pronunciation is null)
        {
            return Result<PronunciationDto>.NotFound(nameof(command.Id), "Pronunciation was not found.");
        }

        var validation = PronunciationInputValidator.Validate(
            command.WordId,
            command.IpaUk,
            command.IpaUs,
            command.ThaiReading,
            command.Syllables,
            command.StressPattern,
            command.MouthPosition,
            command.TonguePosition,
            command.CommonMistake,
            command.PracticeNote,
            command.AudioSlowMediaId,
            command.AudioNormalMediaId,
            command.MouthImageMediaId,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<PronunciationDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await pronunciationRepository.WordHasPronunciationAsync(input.WordId, command.Id, cancellationToken))
        {
            return Result<PronunciationDto>.Validation(
                new ValidationError(nameof(command.WordId), "Word already has a pronunciation record."));
        }

        var referenceValidation = await PronunciationReferenceValidator.ValidateAsync(
            wordRepository,
            mediaRepository,
            input,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<PronunciationDto>.Validation([.. referenceValidation.Errors]);
        }

        pronunciation.Update(
            input.WordId,
            input.IpaUk,
            input.IpaUs,
            input.ThaiReading,
            input.Syllables,
            input.StressPattern,
            input.MouthPosition,
            input.TonguePosition,
            input.CommonMistake,
            input.PracticeNote,
            input.AudioSlowMediaId,
            input.AudioNormalMediaId,
            input.MouthImageMediaId,
            input.IsActive,
            timeProvider.GetUtcNow());

        await pronunciationRepository.SaveChangesAsync(cancellationToken);

        return Result<PronunciationDto>.Success(PronunciationMapper.ToDto(
            pronunciation,
            referenceValidation.Word,
            audioSlowMedia: referenceValidation.AudioSlowMedia,
            audioNormalMedia: referenceValidation.AudioNormalMedia,
            mouthImageMedia: referenceValidation.MouthImageMedia));
    }
}
