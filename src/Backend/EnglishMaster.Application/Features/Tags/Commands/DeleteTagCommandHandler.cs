using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Tags.Commands;

public sealed class DeleteTagCommandHandler
{
    private readonly ITagRepository tagRepository;
    private readonly TimeProvider timeProvider;

    public DeleteTagCommandHandler(
        ITagRepository tagRepository,
        TimeProvider timeProvider)
    {
        this.tagRepository = tagRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteTagCommand command,
        CancellationToken cancellationToken)
    {
        var tag = await tagRepository.GetByIdAsync(command.Id, cancellationToken);
        if (tag is null)
        {
            return Result.NotFound(nameof(command.Id), "Tag was not found.");
        }

        tag.Deactivate(timeProvider.GetUtcNow());
        await tagRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
