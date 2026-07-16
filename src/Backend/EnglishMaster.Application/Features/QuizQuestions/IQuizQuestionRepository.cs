using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.QuizQuestions;

public interface IQuizQuestionRepository
{
    Task AddAsync(QuizQuestion question, CancellationToken cancellationToken);

    Task<QuizQuestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<QuizQuestion>> GetByQuizIdAsync(Guid quizId, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
