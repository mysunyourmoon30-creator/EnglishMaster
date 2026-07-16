using EnglishMaster.Application.Features.QuizQuestions;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Quizzes;

internal sealed class EfQuizQuestionRepository : IQuizQuestionRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfQuizQuestionRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(QuizQuestion question, CancellationToken cancellationToken)
    {
        await dbContext.QuizQuestions.AddAsync(question, cancellationToken);
    }

    public async Task<QuizQuestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.QuizQuestions
            .Include(question => question.Choices)
            .FirstOrDefaultAsync(question => question.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<QuizQuestion>> GetByQuizIdAsync(
        Guid quizId,
        CancellationToken cancellationToken)
    {
        return await dbContext.QuizQuestions
            .Include(question => question.Choices)
            .Where(question => question.QuizId == quizId)
            .OrderBy(question => question.SortOrder)
            .ThenBy(question => question.Id)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
