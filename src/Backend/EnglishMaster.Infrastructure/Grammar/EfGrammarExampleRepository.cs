using EnglishMaster.Application.Features.GrammarExamples;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Grammar;

internal sealed class EfGrammarExampleRepository : IGrammarExampleRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfGrammarExampleRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(GrammarExample grammarExample, CancellationToken cancellationToken)
    {
        await dbContext.GrammarExamples.AddAsync(grammarExample, cancellationToken);
    }

    public async Task<GrammarExample?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.GrammarExamples
            .FirstOrDefaultAsync(grammarExample => grammarExample.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GrammarExample>> GetByGrammarRuleIdAsync(
        Guid grammarRuleId,
        CancellationToken cancellationToken)
    {
        return await dbContext.GrammarExamples
            .AsNoTracking()
            .Where(grammarExample => grammarExample.GrammarRuleId == grammarRuleId)
            .OrderBy(grammarExample => grammarExample.SortOrder)
            .ThenBy(grammarExample => grammarExample.ExampleEn)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
