using EnglishMaster.Application.Features.QuizQuestions;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeQuizQuestionRepository : IQuizQuestionRepository
{
    public List<QuizQuestion> Questions { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(QuizQuestion question, CancellationToken cancellationToken)
    {
        Questions.Add(question);
        return Task.CompletedTask;
    }

    public Task<QuizQuestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Questions.SingleOrDefault(question => question.Id == id));
    }

    public Task<IReadOnlyCollection<QuizQuestion>> GetByQuizIdAsync(
        Guid quizId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<QuizQuestion>>(
            Questions
                .Where(question => question.QuizId == quizId)
                .OrderBy(question => question.SortOrder)
                .ToArray());
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
