using EnglishMaster.Application.Features.Quizzes.Dtos;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.Quizzes;

public interface IQuizRepository
{
    Task AddAsync(Quiz quiz, CancellationToken cancellationToken);

    Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(string slug, Guid? excludedQuizId, CancellationToken cancellationToken);

    Task<QuizSearchResult> SearchAsync(QuizSearchCriteria criteria, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
