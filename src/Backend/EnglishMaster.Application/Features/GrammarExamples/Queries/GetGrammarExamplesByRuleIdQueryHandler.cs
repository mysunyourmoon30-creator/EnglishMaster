using EnglishMaster.Application.Features.GrammarExamples.Dtos;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarExamples.Queries;

public sealed class GetGrammarExamplesByRuleIdQueryHandler
{
    private readonly IGrammarExampleRepository grammarExampleRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;

    public GetGrammarExamplesByRuleIdQueryHandler(
        IGrammarExampleRepository grammarExampleRepository,
        IGrammarRuleRepository grammarRuleRepository)
    {
        this.grammarExampleRepository = grammarExampleRepository;
        this.grammarRuleRepository = grammarRuleRepository;
    }

    public async Task<Result<IReadOnlyCollection<GrammarExampleDto>>> HandleAsync(
        GetGrammarExamplesByRuleIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.GrammarRuleId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<GrammarExampleDto>>.Validation(
                new ValidationError(nameof(query.GrammarRuleId), $"{nameof(query.GrammarRuleId)} cannot be empty."));
        }

        var grammarRule = await grammarRuleRepository.GetByIdAsync(query.GrammarRuleId, cancellationToken);
        if (grammarRule is null)
        {
            return Result<IReadOnlyCollection<GrammarExampleDto>>.NotFound(nameof(query.GrammarRuleId), "Grammar rule was not found.");
        }

        var examples = await grammarExampleRepository.GetByGrammarRuleIdAsync(query.GrammarRuleId, cancellationToken);
        return Result<IReadOnlyCollection<GrammarExampleDto>>.Success(
            examples.Select(GrammarExampleMapper.ToDto).ToArray());
    }
}
