using EnglishMaster.Application.Features.GrammarExamples.Dtos;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarExamples.Commands;

public sealed class AddGrammarExampleCommandHandler
{
    private readonly IGrammarExampleRepository grammarExampleRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly TimeProvider timeProvider;

    public AddGrammarExampleCommandHandler(
        IGrammarExampleRepository grammarExampleRepository,
        IGrammarRuleRepository grammarRuleRepository,
        TimeProvider timeProvider)
    {
        this.grammarExampleRepository = grammarExampleRepository;
        this.grammarRuleRepository = grammarRuleRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<GrammarExampleDto>> HandleAsync(
        AddGrammarExampleCommand command,
        CancellationToken cancellationToken)
    {
        if (command.GrammarRuleId == Guid.Empty)
        {
            return Result<GrammarExampleDto>.Validation(new ValidationError(nameof(command.GrammarRuleId), $"{nameof(command.GrammarRuleId)} cannot be empty."));
        }

        var grammarRule = await grammarRuleRepository.GetByIdAsync(command.GrammarRuleId, cancellationToken);
        if (grammarRule is null)
        {
            return Result<GrammarExampleDto>.NotFound(nameof(command.GrammarRuleId), "Grammar rule was not found.");
        }

        if (!grammarRule.IsActive)
        {
            return Result<GrammarExampleDto>.Validation(new ValidationError(nameof(command.GrammarRuleId), "Grammar rule is inactive."));
        }

        var validation = GrammarExampleInputValidator.Validate(
            command.ExampleEn,
            command.TranslationTh,
            command.ExplanationTh,
            command.IsCorrectExample,
            command.SortOrder,
            isActive: true);
        if (!validation.IsSuccess)
        {
            return Result<GrammarExampleDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var grammarExample = GrammarExample.Create(
            command.GrammarRuleId,
            input.ExampleEn,
            input.TranslationTh,
            input.ExplanationTh,
            input.IsCorrectExample,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await grammarExampleRepository.AddAsync(grammarExample, cancellationToken);
        await grammarExampleRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarExampleDto>.Success(GrammarExampleMapper.ToDto(grammarExample));
    }
}
