using EnglishMaster.Application.Features.QuizChoices.Dtos;
using EnglishMaster.Application.Features.QuizQuestions;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public sealed class UpdateQuizChoiceCommandHandler
{
    private readonly IQuizChoiceRepository choiceRepository;
    private readonly IQuizQuestionRepository questionRepository;
    private readonly TimeProvider timeProvider;

    public UpdateQuizChoiceCommandHandler(
        IQuizChoiceRepository choiceRepository,
        IQuizQuestionRepository questionRepository,
        TimeProvider timeProvider)
    {
        this.choiceRepository = choiceRepository;
        this.questionRepository = questionRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<QuizChoiceDto>> HandleAsync(
        UpdateQuizChoiceCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result<QuizChoiceDto>.Validation(
                new ValidationError(nameof(command.Id), $"{nameof(command.Id)} cannot be empty."));
        }

        var choice = await choiceRepository.GetByIdAsync(command.Id, cancellationToken);
        if (choice is null)
        {
            return Result<QuizChoiceDto>.NotFound(nameof(command.Id), "Quiz choice was not found.");
        }

        var question = await questionRepository.GetByIdAsync(choice.QuizQuestionId, cancellationToken);
        if (question is null)
        {
            return Result<QuizChoiceDto>.NotFound(nameof(choice.QuizQuestionId), "Quiz question was not found.");
        }

        var validation = QuizChoiceInputValidator.Validate(
            command.ChoiceText,
            command.IsCorrect,
            command.ExplanationTh,
            command.ExplanationEn,
            command.SortOrder,
            command.IsActive);
        if (!validation.IsSuccess)
        {
            return Result<QuizChoiceDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        if (input.IsCorrect && !question.CanAddCorrectChoice(choice.Id))
        {
            return Result<QuizChoiceDto>.Validation(
                new ValidationError(nameof(command.IsCorrect), $"{question.QuestionType} questions can have only one correct choice."));
        }

        choice.Update(
            input.ChoiceText,
            input.IsCorrect,
            input.ExplanationTh,
            input.ExplanationEn,
            input.SortOrder,
            input.IsActive,
            timeProvider.GetUtcNow());

        await choiceRepository.SaveChangesAsync(cancellationToken);

        return Result<QuizChoiceDto>.Success(QuizChoiceMapper.ToDto(choice));
    }
}
