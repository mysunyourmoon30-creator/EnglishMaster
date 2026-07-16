using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed class ActivateGrammarRuleCommandHandler
{
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly IWordRepository wordRepository;
    private readonly TimeProvider timeProvider;

    public ActivateGrammarRuleCommandHandler(
        IGrammarRuleRepository grammarRuleRepository,
        IGrammarTopicRepository grammarTopicRepository,
        IWordRepository wordRepository,
        TimeProvider timeProvider)
    {
        this.grammarRuleRepository = grammarRuleRepository;
        this.grammarTopicRepository = grammarTopicRepository;
        this.wordRepository = wordRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<GrammarRuleDto>> HandleAsync(
        ActivateGrammarRuleCommand command,
        CancellationToken cancellationToken)
    {
        var grammarRule = await grammarRuleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (grammarRule is null)
        {
            return Result<GrammarRuleDto>.NotFound(nameof(command.Id), "Grammar rule was not found.");
        }

        grammarRule.Activate(timeProvider.GetUtcNow());
        await grammarRuleRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarRuleDto>.Success(await GrammarRuleReadModelBuilder.MapAsync(
            grammarRule,
            grammarTopicRepository,
            wordRepository,
            cancellationToken));
    }
}
