using EnglishMaster.Application.Features.QuizChoices;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeQuizChoiceRepository : IQuizChoiceRepository
{
    public List<QuizChoice> Choices { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(QuizChoice choice, CancellationToken cancellationToken)
    {
        Choices.Add(choice);
        return Task.CompletedTask;
    }

    public Task<QuizChoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Choices.SingleOrDefault(choice => choice.Id == id));
    }

    public Task<IReadOnlyCollection<QuizChoice>> GetByQuestionIdAsync(
        Guid questionId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<QuizChoice>>(
            Choices
                .Where(choice => choice.QuizQuestionId == questionId)
                .OrderBy(choice => choice.SortOrder)
                .ToArray());
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
