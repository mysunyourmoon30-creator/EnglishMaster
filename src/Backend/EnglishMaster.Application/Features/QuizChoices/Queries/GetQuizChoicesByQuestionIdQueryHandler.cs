using EnglishMaster.Application.Features.QuizChoices.Dtos;
using EnglishMaster.Application.Features.QuizQuestions;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Queries;

public sealed class GetQuizChoicesByQuestionIdQueryHandler
{
    private readonly IQuizChoiceRepository choiceRepository;
    private readonly IQuizQuestionRepository questionRepository;

    public GetQuizChoicesByQuestionIdQueryHandler(
        IQuizChoiceRepository choiceRepository,
        IQuizQuestionRepository questionRepository)
    {
        this.choiceRepository = choiceRepository;
        this.questionRepository = questionRepository;
    }

    public async Task<Result<IReadOnlyCollection<QuizChoiceDto>>> HandleAsync(
        GetQuizChoicesByQuestionIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.QuizQuestionId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.Validation(
                new ValidationError(nameof(query.QuizQuestionId), $"{nameof(query.QuizQuestionId)} cannot be empty."));
        }

        var question = await questionRepository.GetByIdAsync(query.QuizQuestionId, cancellationToken);
        if (question is null)
        {
            return Result<IReadOnlyCollection<QuizChoiceDto>>.NotFound(nameof(query.QuizQuestionId), "Quiz question was not found.");
        }

        var choices = await choiceRepository.GetByQuestionIdAsync(query.QuizQuestionId, cancellationToken);
        return Result<IReadOnlyCollection<QuizChoiceDto>>.Success(QuizChoiceMapper.ToDtos(choices));
    }
}
