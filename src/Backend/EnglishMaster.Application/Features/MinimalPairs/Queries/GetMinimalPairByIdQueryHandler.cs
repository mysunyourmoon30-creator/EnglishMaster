using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.MinimalPairs.Dtos;
using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.MinimalPairs.Queries;

public sealed class GetMinimalPairByIdQueryHandler
{
    private readonly IMinimalPairRepository minimalPairRepository;
    private readonly IMediaRepository mediaRepository;

    public GetMinimalPairByIdQueryHandler(
        IMinimalPairRepository minimalPairRepository,
        IMediaRepository mediaRepository)
    {
        this.minimalPairRepository = minimalPairRepository;
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<MinimalPairDto>> HandleAsync(
        GetMinimalPairByIdQuery query,
        CancellationToken cancellationToken)
    {
        var minimalPair = await minimalPairRepository.GetByIdAsync(query.Id, cancellationToken);
        if (minimalPair is null)
        {
            return Result<MinimalPairDto>.NotFound(nameof(query.Id), "Minimal pair was not found.");
        }

        var audioMedia = minimalPair.AudioMediaId.HasValue
            ? await mediaRepository.GetByIdAsync(minimalPair.AudioMediaId.Value, cancellationToken)
            : null;

        return Result<MinimalPairDto>.Success(MinimalPairMapper.ToDto(minimalPair, audioMedia));
    }
}
