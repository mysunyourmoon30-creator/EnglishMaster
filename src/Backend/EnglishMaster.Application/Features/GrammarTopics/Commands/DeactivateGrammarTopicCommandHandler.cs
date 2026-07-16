using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarTopics.Commands;

public sealed class DeactivateGrammarTopicCommandHandler
{
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly TimeProvider timeProvider;

    public DeactivateGrammarTopicCommandHandler(
        IGrammarTopicRepository grammarTopicRepository,
        TimeProvider timeProvider)
    {
        this.grammarTopicRepository = grammarTopicRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<GrammarTopicDto>> HandleAsync(
        DeactivateGrammarTopicCommand command,
        CancellationToken cancellationToken)
    {
        var grammarTopic = await grammarTopicRepository.GetByIdAsync(command.Id, cancellationToken);
        if (grammarTopic is null)
        {
            return Result<GrammarTopicDto>.NotFound(nameof(command.Id), "Grammar topic was not found.");
        }

        grammarTopic.Deactivate(timeProvider.GetUtcNow());
        await grammarTopicRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarTopicDto>.Success(GrammarTopicMapper.ToDto(grammarTopic));
    }
}
