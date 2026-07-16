using EnglishMaster.Application.Features.GrammarExamples.Dtos;
using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarExamples.Commands;

public sealed class UpdateGrammarExampleCommandHandler
{
    private readonly IGrammarExampleRepository grammarExampleRepository;
    private readonly TimeProvider timeProvider;

    public UpdateGrammarExampleCommandHandler(
        IGrammarExampleRepository grammarExampleRepository,
        TimeProvider timeProvider)
    {
        this.grammarExampleRepository = grammarExampleRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<GrammarExampleDto>> HandleAsync(
        UpdateGrammarExampleCommand command,
        CancellationToken cancellationToken)
    {
        var grammarExample = await grammarExampleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (grammarExample is null)
        {
            return Result<GrammarExampleDto>.NotFound(nameof(command.Id), "Grammar example was not found.");
        }

        var validation = GrammarExampleInputValidator.Validate(
            command.ExampleEn,
            command.TranslationTh,
            command.ExplanationTh,
            command.IsCorrectExample,
            command.SortOrder,
            command.IsActive);
        if (!validation.IsSuccess)
        {
            return Result<GrammarExampleDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        grammarExample.Update(
            input.ExampleEn,
            input.TranslationTh,
            input.ExplanationTh,
            input.IsCorrectExample,
            input.SortOrder,
            input.IsActive,
            timeProvider.GetUtcNow());

        await grammarExampleRepository.SaveChangesAsync(cancellationToken);

        return Result<GrammarExampleDto>.Success(GrammarExampleMapper.ToDto(grammarExample));
    }
}
