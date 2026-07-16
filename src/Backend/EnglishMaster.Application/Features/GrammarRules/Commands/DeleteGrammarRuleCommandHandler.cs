using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed class DeleteGrammarRuleCommandHandler
{
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly TimeProvider timeProvider;

    public DeleteGrammarRuleCommandHandler(
        IGrammarRuleRepository grammarRuleRepository,
        TimeProvider timeProvider)
    {
        this.grammarRuleRepository = grammarRuleRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        DeleteGrammarRuleCommand command,
        CancellationToken cancellationToken)
    {
        var grammarRule = await grammarRuleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (grammarRule is null)
        {
            return Result.NotFound(nameof(command.Id), "Grammar rule was not found.");
        }

        grammarRule.Deactivate(timeProvider.GetUtcNow());
        await grammarRuleRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
