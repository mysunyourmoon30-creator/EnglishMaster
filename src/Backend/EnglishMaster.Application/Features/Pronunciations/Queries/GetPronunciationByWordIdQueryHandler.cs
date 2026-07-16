using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Queries;

public sealed class GetPronunciationByWordIdQueryHandler
{
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly IWordRepository wordRepository;
    private readonly IMediaRepository mediaRepository;

    public GetPronunciationByWordIdQueryHandler(
        IPronunciationRepository pronunciationRepository,
        IWordRepository wordRepository,
        IMediaRepository mediaRepository)
    {
        this.pronunciationRepository = pronunciationRepository;
        this.wordRepository = wordRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<PronunciationDto>> HandleAsync(
        GetPronunciationByWordIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.WordId == Guid.Empty)
        {
            return Result<PronunciationDto>.Validation(
                new ValidationError(nameof(query.WordId), $"{nameof(query.WordId)} cannot be empty."));
        }

        var pronunciation = await pronunciationRepository.GetByWordIdAsync(query.WordId, cancellationToken);
        if (pronunciation is null)
        {
            return Result<PronunciationDto>.NotFound(nameof(query.WordId), "Pronunciation was not found.");
        }

        var dto = await PronunciationReadModelBuilder.MapAsync(
            pronunciation,
            wordRepository,
            mediaRepository,
            includeMinimalPairs: true,
            cancellationToken);

        return Result<PronunciationDto>.Success(dto);
    }
}
