using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.QuizChoices;

public interface IQuizChoiceRepository
{
    Task AddAsync(QuizChoice choice, CancellationToken cancellationToken);

    Task<QuizChoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<QuizChoice>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
