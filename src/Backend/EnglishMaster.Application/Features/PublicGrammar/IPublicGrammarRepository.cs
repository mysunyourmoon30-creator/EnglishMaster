using EnglishMaster.Application.Features.PublicGrammar.Dtos;

namespace EnglishMaster.Application.Features.PublicGrammar;

public interface IPublicGrammarRepository
{
    Task<PublicGrammarTopicDetailDto?> GetTopicBySlugAsync(
        string slug,
        CancellationToken cancellationToken);

    Task<PublicGrammarRuleDetailDto?> GetRuleBySlugAsync(
        string slug,
        CancellationToken cancellationToken);
}