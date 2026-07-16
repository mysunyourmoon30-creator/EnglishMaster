using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed class AddRelatedWordToGrammarRuleCommandHandler
{
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly IWordRepository wordRepository;
    private readonly TimeProvider timeProvider;

    public AddRelatedWordToGrammarRuleCommandHandler(
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
        AddRelatedWordToGrammarRuleCommand command,
        CancellationToken cancellationToken)
    {
        if (command.GrammarRuleId == Guid.Empty)
        {
            return Result<GrammarRuleDto>.Validation(new ValidationError(nameof(command.GrammarRuleId), $"{nameof(command.GrammarRuleId)} cannot be empty."));
        }

        if (command.WordId == Guid.Empty)
        {
            return Result<GrammarRuleDto>.Validation(new ValidationError(nameof(command.WordId), $"{nameof(command.WordId)} cannot be empty."));
        }

        var grammarRule = await grammarRuleRepository.GetByIdAsync(command.GrammarRuleId, cancellationToken);
        if (grammarRule is null)
        {
            return Result<GrammarRuleDto>.NotFound(nameof(command.GrammarRuleId), "Grammar rule was not found.");
        }

        if (!grammarRule.IsActive)
        {
            return Result<GrammarRuleDto>.Validation(new ValidationError(nameof(command.GrammarRuleId), "Grammar rule is inactive."));
        }

        var wordValidation = await GrammarRuleReferenceValidator.ValidateWordAsync(
            wordRepository,
            command.WordId,
            cancellationToken);
        if (wordValidation.Errors.Count > 0)
        {
            return Result<GrammarRuleDto>.Validation([.. wordValidation.Errors]);
        }

        grammarRule.AddRelatedWord(command.WordId, timeProvider.GetUtcNow());
        await grammarRuleRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarRuleDto>.Success(await GrammarRuleReadModelBuilder.MapAsync(
            grammarRule,
            grammarTopicRepository,
            wordRepository,
            cancellationToken));
    }
}
