using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Media.Queries;

public sealed class GetMediaByIdQueryHandler
{
    private readonly IMediaRepository mediaRepository;

    public GetMediaByIdQueryHandler(IMediaRepository mediaRepository)
    {
        this.mediaRepository = mediaRepository;
    }

    public async Task<Result<MediaDto>> HandleAsync(
        GetMediaByIdQuery query,
        CancellationToken cancellationToken)
    {
        var media = await mediaRepository.GetByIdAsync(query.Id, cancellationToken);
        return media is null
            ? Result<MediaDto>.NotFound(nameof(query.Id), "Media was not found.")
            : Result<MediaDto>.Success(MediaMapper.ToDto(media));
    }
}
