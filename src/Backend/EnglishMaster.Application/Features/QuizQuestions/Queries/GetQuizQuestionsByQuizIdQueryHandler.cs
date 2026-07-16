using EnglishMaster.Application.Features.QuizQuestions.Dtos;
using EnglishMaster.Application.Features.Quizzes;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Queries;

public sealed class GetQuizQuestionsByQuizIdQueryHandler
{
    private readonly IQuizQuestionRepository questionRepository;
    private readonly IQuizRepository quizRepository;

    public GetQuizQuestionsByQuizIdQueryHandler(
        IQuizQuestionRepository questionRepository,
        IQuizRepository quizRepository)
    {
        this.questionRepository = questionRepository;
        this.quizRepository = quizRepository;
    }

    public async Task<Result<IReadOnlyCollection<QuizQuestionDto>>> HandleAsync(
        GetQuizQuestionsByQuizIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.QuizId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.Validation(
                new ValidationError(nameof(query.QuizId), $"{nameof(query.QuizId)} cannot be empty."));
        }

        var quiz = await quizRepository.GetByIdAsync(query.QuizId, cancellationToken);
        if (quiz is null)
        {
            return Result<IReadOnlyCollection<QuizQuestionDto>>.NotFound(nameof(query.QuizId), "Quiz was not found.");
        }

        var questions = await questionRepository.GetByQuizIdAsync(query.QuizId, cancellationToken);
        return Result<IReadOnlyCollection<QuizQuestionDto>>.Success(QuizQuestionReadModelBuilder.Map(questions));
    }
}
