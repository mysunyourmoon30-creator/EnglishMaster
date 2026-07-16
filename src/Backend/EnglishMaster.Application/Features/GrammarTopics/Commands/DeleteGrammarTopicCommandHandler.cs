using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarTopics.Commands;

public sealed class DeleteGrammarTopicCommandHandler
{
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly TimeProvider timeProvider;

    public DeleteGrammarTopicCommandHandler(
        IGrammarTopicRepository grammarTopicRepository,
        TimeProvider timeProvider)
    {
        this.grammarTopicRepository = grammarTopicRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteGrammarTopicCommand command,
        CancellationToken cancellationToken)
    {
        var grammarTopic = await grammarTopicRepository.GetByIdAsync(command.Id, cancellationToken);
        if (grammarTopic is null)
        {
            return Result.NotFound(nameof(command.Id), "Grammar topic was not found.");
        }

        grammarTopic.Deactivate(timeProvider.GetUtcNow());
        await grammarTopicRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
