using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.Application.Features.GrammarExamples;

public interface IGrammarExampleRepository
{
    Task AddAsync(GrammarExample grammarExample, CancellationToken cancellationToken);

    Task<GrammarExample?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GrammarExample>> GetByGrammarRuleIdAsync(
        Guid grammarRuleId,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
