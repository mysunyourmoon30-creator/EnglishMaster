using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.Application.Features.GrammarTopics;

public interface IGrammarTopicRepository
{
    Task AddAsync(GrammarTopic grammarTopic, CancellationToken cancellationToken);

    Task<GrammarTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GrammarTopic>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedGrammarTopicId,
        CancellationToken cancellationToken);

    Task<GrammarTopicSearchResult> SearchAsync(
        GrammarTopicSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
