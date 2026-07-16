using EnglishMaster.Application.Features.QuizChoices;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Quizzes;

internal sealed class EfQuizChoiceRepository : IQuizChoiceRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfQuizChoiceRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(QuizChoice choice, CancellationToken cancellationToken)
    {
        await dbContext.QuizChoices.AddAsync(choice, cancellationToken);
    }

    public async Task<QuizChoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.QuizChoices
            .FirstOrDefaultAsync(choice => choice.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<QuizChoice>> GetByQuestionIdAsync(
        Guid questionId,
        CancellationToken cancellationToken)
    {
        return await dbContext.QuizChoices
            .Where(choice => choice.QuizQuestionId == questionId)
            .OrderBy(choice => choice.SortOrder)
            .ThenBy(choice => choice.Id)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
