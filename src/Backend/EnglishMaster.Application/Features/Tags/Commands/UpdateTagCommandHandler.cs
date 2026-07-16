using EnglishMaster.Application.Features.Tags.Dtos;
using EnglishMaster.Contracts.Tags;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Tags.Commands;

public sealed class UpdateTagCommandHandler
{
    private readonly ITagRepository tagRepository;
    private readonly TimeProvider timeProvider;

    public UpdateTagCommandHandler(
        ITagRepository tagRepository,
        TimeProvider timeProvider)
    {
        this.tagRepository = tagRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<TagDto>> HandleAsync(
        UpdateTagCommand command,
        CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByIdAsync(command.Id, cancellationToken);
        if (tag is null)
        {
            return Result<TagDto>.NotFound(nameof(command.Id), "Tag was not found.");
        }

        var validation = TagInputValidator.Validate(
            command.Name,
            command.Description,
            command.IsActive);

        if (!validation.IsSuccess)
        {
            return Result<TagDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await tagRepository.SlugExistsAsync(input.Slug, command.Id, cancellationToken))
        {
            return Result<TagDto>.Validation(
                new ValidationError(nameof(command.Name), "A tag with this name already exists."));
        }

        tag.Update(
            input.Name,
            input.Description,
            input.IsActive,
            timeProvider.GetUtcNow());

        await tagRepository.SaveChangesAsync(cancellationToken);

        return Result<TagDto>.Success(TagMapper.ToDto(tag));
    }
}
