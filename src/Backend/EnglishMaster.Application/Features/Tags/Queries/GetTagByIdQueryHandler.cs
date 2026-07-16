using EnglishMaster.Application.Features.Tags.Dtos;
using EnglishMaster.Contracts.Tags;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Tags.Queries;

public sealed class GetTagByIdQueryHandler
{
    private readonly ITagRepository tagRepository;

    public GetTagByIdQueryHandler(ITagRepository tagRepository)
    {
        this.tagRepository = tagRepository;
    }

    public async Task<Result<TagDto>> HandleAsync(
        GetTagByIdQuery query,
        CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByIdAsync(query.Id, cancellationToken);
        return tag is null
            ? Result<TagDto>.NotFound(nameof(query.Id), "Tag was not found.")
            : Result<TagDto>.Success(TagMapper.ToDto(tag));
    }
}
