using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Commands;

public sealed class CreatePronunciationCommandHandler
{
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly IWordRepository wordRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly TimeProvider timeProvider;

    public CreatePronunciationCommandHandler(
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
        CreatePronunciationCommand command,
        CancellationToken cancellationToken)
    {
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
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<PronunciationDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await pronunciationRepository.WordHasPronunciationAsync(input.WordId, null, cancellationToken))
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

        var now = timeProvider.GetUtcNow();
        var pronunciation = Pronunciation.Create(
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
            now);

        await pronunciationRepository.AddAsync(pronunciation, cancellationToken);
        await pronunciationRepository.SaveChangesAsync(cancellationToken);

        return Result<PronunciationDto>.Success(PronunciationMapper.ToDto(
            pronunciation,
            referenceValidation.Word,
            audioSlowMedia: referenceValidation.AudioSlowMedia,
            audioNormalMedia: referenceValidation.AudioNormalMedia,
            mouthImageMedia: referenceValidation.MouthImageMedia));
    }
}
