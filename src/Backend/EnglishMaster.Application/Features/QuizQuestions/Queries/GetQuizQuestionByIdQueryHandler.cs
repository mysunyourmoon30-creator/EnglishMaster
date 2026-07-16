using EnglishMaster.Application.Features.QuizQuestions.Dtos;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Queries;

public sealed class GetQuizQuestionByIdQueryHandler
{
    private readonly IQuizQuestionRepository questionRepository;

    public GetQuizQuestionByIdQueryHandler(IQuizQuestionRepository questionRepository)
    {
        this.questionRepository = questionRepository;
    }

    public async Task<Result<QuizQuestionDto>> HandleAsync(
        GetQuizQuestionByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
        {
            return Result<QuizQuestionDto>.Validation(
                new ValidationError(nameof(query.Id), $"{nameof(query.Id)} cannot be empty."));
        }

        var question = await questionRepository.GetByIdAsync(query.Id, cancellationToken);
        return question is null
            ? Result<QuizQuestionDto>.NotFound(nameof(query.Id), "Quiz question was not found.")
            : Result<QuizQuestionDto>.Success(QuizQuestionReadModelBuilder.Map(question));
    }
}
