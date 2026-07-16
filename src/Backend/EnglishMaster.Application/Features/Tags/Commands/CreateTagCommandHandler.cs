using EnglishMaster.Application.Features.Tags.Dtos;
using EnglishMaster.Contracts.Tags;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Tags.Commands;

public sealed class CreateTagCommandHandler
{
    private readonly ITagRepository tagRepository;
    private readonly TimeProvider timeProvider;

    public CreateTagCommandHandler(
        ITagRepository tagRepository,
        TimeProvider timeProvider)
    {
        this.tagRepository = tagRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<TagDto>> HandleAsync(
        CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var validation = TagInputValidator.Validate(
            command.Name,
            command.Description,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<TagDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (await tagRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<TagDto>.Validation(
                new ValidationError(nameof(command.Name), "A tag with this name already exists."));
        }

        var tag = Tag.Create(
            input.Name,
            input.Description,
            timeProvider.GetUtcNow());

        await tagRepository.AddAsync(tag, cancellationToken);
        await tagRepository.SaveChangesAsync(cancellationToken);

        return Result<TagDto>.Success(TagMapper.ToDto(tag));
    }
}
