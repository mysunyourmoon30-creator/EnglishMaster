using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Pronunciations.Queries;

public sealed class GetPronunciationByIdQueryHandler
{
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly IWordRepository wordRepository;
    private readonly IMediaRepository mediaRepository;

    public GetPronunciationByIdQueryHandler(
        IPronunciationRepository pronunciationRepository,
        IWordRepository wordRepository,
        IMediaRepository mediaRepository)
    {
        this.pronunciationRepository = pronunciationRepository;
        this.wordRepository = wordRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<PronunciationDto>> HandleAsync(
        GetPronunciationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var pronunciation = await pronunciationRepository.GetByIdAsync(query.Id, cancellationToken);
        if (pronunciation is null)
        {
            return Result<PronunciationDto>.NotFound(nameof(query.Id), "Pronunciation was not found.");
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
