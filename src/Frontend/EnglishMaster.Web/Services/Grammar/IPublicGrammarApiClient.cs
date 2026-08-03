using EnglishMaster.Contracts.PublicGrammar;

namespace EnglishMaster.Web.Services.Grammar;

public interface IPublicGrammarApiClient
{
    Task<PublicGrammarTopicDetailDto?> GetTopicBySlugAsync(
        string slug,
        CancellationToken cancellationToken);

    Task<PublicGrammarRuleDetailDto?> GetRuleBySlugAsync(
        string slug,
        CancellationToken cancellationToken);
}