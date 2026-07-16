using EnglishMaster.Application.Features.QuizChoices.Dtos;
using EnglishMaster.Application.Features.QuizQuestions;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public sealed class AddQuizChoiceCommandHandler
{
    private readonly IQuizChoiceRepository choiceRepository;
    private readonly IQuizQuestionRepository questionRepository;
    private readonly TimeProvider timeProvider;

    public AddQuizChoiceCommandHandler(
        IQuizChoiceRepository choiceRepository,
        IQuizQuestionRepository questionRepository,
        TimeProvider timeProvider)
    {
        this.choiceRepository = choiceRepository;
        this.questionRepository = questionRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<QuizChoiceDto>> HandleAsync(
        AddQuizChoiceCommand command,
        CancellationToken cancellationToken)
    {
        if (command.QuizQuestionId == Guid.Empty)
        {
            return Result<QuizChoiceDto>.Validation(
                new ValidationError(nameof(command.QuizQuestionId), $"{nameof(command.QuizQuestionId)} cannot be empty."));
        }

        var question = await questionRepository.GetByIdAsync(command.QuizQuestionId, cancellationToken);
        if (question is null)
        {
            return Result<QuizChoiceDto>.NotFound(nameof(command.QuizQuestionId), "Quiz question was not found.");
        }

        if (!question.IsActive)
        {
            return Result<QuizChoiceDto>.Validation(
                new ValidationError(nameof(command.QuizQuestionId), "Quiz question is inactive."));
        }

        var validation = QuizChoiceInputValidator.Validate(
            command.ChoiceText,
            command.IsCorrect,
            command.ExplanationTh,
            command.ExplanationEn,
            command.SortOrder,
            isActive: true);
        if (!validation.IsSuccess)
        {
            return Result<QuizChoiceDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (input.IsCorrect && !question.CanAddCorrectChoice())
        {
            return Result<QuizChoiceDto>.Validation(
                new ValidationError(nameof(command.IsCorrect), $"{question.QuestionType} questions can have only one correct choice."));
        }

        var choice = question.AddChoice(
            input.ChoiceText,
            input.IsCorrect,
            input.ExplanationTh,
            input.ExplanationEn,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await choiceRepository.AddAsync(choice, cancellationToken);
        await choiceRepository.SaveChangesAsync(cancellationToken);

        return Result<QuizChoiceDto>.Success(QuizChoiceMapper.ToDto(choice));
    }
}
