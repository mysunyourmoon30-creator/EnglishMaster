using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed class RemoveRelatedWordFromGrammarRuleCommandHandler
{
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly TimeProvider timeProvider;

    public RemoveRelatedWordFromGrammarRuleCommandHandler(
        IGrammarRuleRepository grammarRuleRepository,
        TimeProvider timeProvider)
    {
        this.grammarRuleRepository = grammarRuleRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result> HandleAsync(
        RemoveRelatedWordFromGrammarRuleCommand command,
        CancellationToken cancellationToken)
    {
        if (command.GrammarRuleId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.GrammarRuleId), $"{nameof(command.GrammarRuleId)} cannot be empty."));
        }

        if (command.WordId == Guid.Empty)
        {
            return Result.Validation(new ValidationError(nameof(command.WordId), $"{nameof(command.WordId)} cannot be empty."));
        }

        var grammarRule = await grammarRuleRepository.GetByIdAsync(command.GrammarRuleId, cancellationToken);
        if (grammarRule is null)
        {
            return Result.NotFound(nameof(command.GrammarRuleId), "Grammar rule was not found.");
        }

        if (!grammarRule.RemoveRelatedWord(command.WordId, timeProvider.GetUtcNow()))
        {
            return Result.NotFound(nameof(command.WordId), "Related word was not found on this grammar rule.");
        }

        await grammarRuleRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
