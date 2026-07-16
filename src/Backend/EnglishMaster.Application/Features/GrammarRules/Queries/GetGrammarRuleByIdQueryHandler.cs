using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Queries;

public sealed class GetGrammarRuleByIdQueryHandler
{
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly IWordRepository wordRepository;

    public GetGrammarRuleByIdQueryHandler(
        IGrammarRuleRepository grammarRuleRepository,
        IGrammarTopicRepository grammarTopicRepository,
        IWordRepository wordRepository)
    {
        this.grammarRuleRepository = grammarRuleRepository;
        this.grammarTopicRepository = grammarTopicRepository;
        this.wordRepository = wordRepository;
    }

    public async Task<Result<GrammarRuleDto>> HandleAsync(
        GetGrammarRuleByIdQuery query,
        CancellationToken cancellationToken)
    {
        var grammarRule = await grammarRuleRepository.GetByIdAsync(query.Id, cancellationToken);
        if (grammarRule is null)
        {
            return Result<GrammarRuleDto>.NotFound(nameof(query.Id), "Grammar rule was not found.");
        }

        return Result<GrammarRuleDto>.Success(await GrammarRuleReadModelBuilder.MapAsync(
            grammarRule,
            grammarTopicRepository,
            wordRepository,
            cancellationToken));
    }
}
