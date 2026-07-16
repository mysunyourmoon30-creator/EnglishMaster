using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Contracts.GrammarTopics;

namespace EnglishMaster.Web.Services.Grammar;

public interface IGrammarApiClient
{
    Task<GrammarTopicSearchResponse> SearchTopicsAsync(
        GrammarTopicSearchRequest request,
        CancellationToken cancellationToken);

    Task<GrammarTopicDto?> GetTopicAsync(Guid id, CancellationToken cancellationToken);

    Task<GrammarTopicDto> CreateTopicAsync(
        CreateGrammarTopicRequest request,
        CancellationToken cancellationToken);

    Task<GrammarTopicDto> UpdateTopicAsync(
        Guid id,
        UpdateGrammarTopicRequest request,
        CancellationToken cancellationToken);

    Task DeleteTopicAsync(Guid id, CancellationToken cancellationToken);

    Task<GrammarRuleSearchResponse> SearchRulesAsync(
        GrammarRuleSearchRequest request,
        CancellationToken cancellationToken);

    Task<GrammarRuleSearchResponse> GetRulesByTopicIdAsync(
        Guid topicId,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<GrammarRuleDto?> GetRuleAsync(Guid id, CancellationToken cancellationToken);

    Task<GrammarRuleDto> CreateRuleAsync(
        CreateGrammarRuleRequest request,
        CancellationToken cancellationToken);

    Task<GrammarRuleDto> UpdateRuleAsync(
        Guid id,
        UpdateGrammarRuleRequest request,
        CancellationToken cancellationToken);

    Task DeleteRuleAsync(Guid id, CancellationToken cancellationToken);

    Task<GrammarRuleDto> AddRelatedWordAsync(
        Guid grammarRuleId,
        Guid wordId,
        CancellationToken cancellationToken);

    Task RemoveRelatedWordAsync(
        Guid grammarRuleId,
        Guid wordId,
        CancellationToken cancellationToken);

    Task<GrammarExampleDto?> GetExampleAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GrammarExampleDto>> GetExamplesByRuleIdAsync(
        Guid grammarRuleId,
        CancellationToken cancellationToken);

    Task<GrammarExampleDto> AddExampleAsync(
        Guid grammarRuleId,
        CreateGrammarExampleRequest request,
        CancellationToken cancellationToken);

    Task<GrammarExampleDto> UpdateExampleAsync(
        Guid id,
        UpdateGrammarExampleRequest request,
        CancellationToken cancellationToken);

    Task DeleteExampleAsync(Guid id, CancellationToken cancellationToken);
}
