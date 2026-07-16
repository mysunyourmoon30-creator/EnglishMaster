using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Commands;

public sealed class CreateGrammarRuleCommandHandler
{
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly IWordRepository wordRepository;
    private readonly TimeProvider timeProvider;

    public CreateGrammarRuleCommandHandler(
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
        CreateGrammarRuleCommand command,
        CancellationToken cancellationToken)
    {
        var validation = GrammarRuleInputValidator.Validate(
            command.GrammarTopicId,
            command.Title,
            command.RuleText,
            command.ExplanationTh,
            command.ExplanationEn,
            command.StructurePattern,
            command.CommonMistake,
            command.CorrectUsageNote,
            command.SortOrder,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<GrammarRuleDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceValidation = await GrammarRuleReferenceValidator.ValidateTopicAsync(
            grammarTopicRepository,
            input.GrammarTopicId,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<GrammarRuleDto>.Validation([.. referenceValidation.Errors]);
        }

        if (await grammarRuleRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<GrammarRuleDto>.Validation(
                new ValidationError(nameof(command.Title), "A grammar rule with this title already exists."));
        }

        var grammarRule = GrammarRule.Create(
            input.GrammarTopicId,
            input.Title,
            input.RuleText,
            input.ExplanationTh,
            input.ExplanationEn,
            input.StructurePattern,
            input.CommonMistake,
            input.CorrectUsageNote,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await grammarRuleRepository.AddAsync(grammarRule, cancellationToken);
        await grammarRuleRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarRuleDto>.Success(await GrammarRuleReadModelBuilder.MapAsync(
            grammarRule,
            grammarTopicRepository,
            wordRepository,
            cancellationToken));
    }
}
