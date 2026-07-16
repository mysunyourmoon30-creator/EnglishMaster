using EnglishMaster.Application.Features.GrammarExamples;
using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeGrammarExampleRepository : IGrammarExampleRepository
{
    public List<GrammarExample> GrammarExamples { get; } = [];

    public Task AddAsync(GrammarExample grammarExample, CancellationToken cancellationToken)
    {
        GrammarExamples.Add(grammarExample);
        return Task.CompletedTask;
    }

    public Task<GrammarExample?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(GrammarExamples.SingleOrDefault(example => example.Id == id));
    }

    public Task<IReadOnlyCollection<GrammarExample>> GetByGrammarRuleIdAsync(
        Guid grammarRuleId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<GrammarExample>>(
            GrammarExamples
                .Where(example => example.GrammarRuleId == grammarRuleId)
                .OrderBy(example => example.SortOrder)
                .ThenBy(example => example.ExampleEn)
                .ToArray());
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1);
    }
}
