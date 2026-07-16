using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Contracts.Quizzes;

namespace EnglishMaster.Web.Services.Quizzes;

public interface IQuizApiClient
{
    Task<QuizSearchResponse> SearchAsync(QuizSearchRequest request, CancellationToken cancellationToken);

    Task<QuizDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<QuizDto> CreateAsync(CreateQuizRequest request, CancellationToken cancellationToken);

    Task<QuizDto> UpdateAsync(Guid id, UpdateQuizRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<QuizDto> PublishAsync(Guid id, CancellationToken cancellationToken);

    Task<QuizDto> UnpublishAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<QuizQuestionDto>> GetQuestionsAsync(Guid quizId, CancellationToken cancellationToken);

    Task<QuizQuestionDto> AddQuestionAsync(
        Guid quizId,
        CreateQuizQuestionRequest request,
        CancellationToken cancellationToken);

    Task<QuizQuestionDto> UpdateQuestionAsync(
        Guid id,
        UpdateQuizQuestionRequest request,
        CancellationToken cancellationToken);

    Task DeleteQuestionAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<QuizQuestionDto>> ReorderQuestionsAsync(
        Guid quizId,
        IReadOnlyList<Guid> orderedQuestionIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<QuizChoiceDto>> GetChoicesAsync(Guid questionId, CancellationToken cancellationToken);

    Task<QuizChoiceDto> AddChoiceAsync(
        Guid questionId,
        CreateQuizChoiceRequest request,
        CancellationToken cancellationToken);

    Task<QuizChoiceDto> UpdateChoiceAsync(
        Guid id,
        UpdateQuizChoiceRequest request,
        CancellationToken cancellationToken);

    Task DeleteChoiceAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<QuizChoiceDto>> ReorderChoicesAsync(
        Guid questionId,
        IReadOnlyList<Guid> orderedChoiceIds,
        CancellationToken cancellationToken);
}
