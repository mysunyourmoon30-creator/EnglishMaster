using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Application.Features.QuizQuestions.Dtos;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public sealed class UpdateQuizQuestionCommandHandler
{
    private readonly IQuizQuestionRepository questionRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly TimeProvider timeProvider;

    public UpdateQuizQuestionCommandHandler(
        IQuizQuestionRepository questionRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository,
        IPronunciationRepository pronunciationRepository,
        TimeProvider timeProvider)
    {
        this.questionRepository = questionRepository;
        this.wordRepository = wordRepository;
        this.grammarRuleRepository = grammarRuleRepository;
        this.pronunciationRepository = pronunciationRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<QuizQuestionDto>> HandleAsync(
        UpdateQuizQuestionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Result<QuizQuestionDto>.Validation(
                new ValidationError(nameof(command.Id), $"{nameof(command.Id)} cannot be empty."));
        }

        var question = await questionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (question is null)
        {
            return Result<QuizQuestionDto>.NotFound(nameof(command.Id), "Quiz question was not found.");
        }

        var validation = QuizQuestionInputValidator.Validate(
            command.QuestionText,
            command.QuestionType,
            command.ExplanationTh,
            command.ExplanationEn,
            command.Points,
            command.SortOrder,
            command.WordId,
            command.GrammarRuleId,
            command.PronunciationId,
            command.IsActive);
        if (!validation.IsSuccess)
        {
            return Result<QuizQuestionDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceErrors = await QuizQuestionReferenceValidator.ValidateReferencesAsync(
            wordRepository,
            grammarRuleRepository,
            pronunciationRepository,
            input,
            cancellationToken);
        if (referenceErrors.Count > 0)
        {
            return Result<QuizQuestionDto>.Validation([.. referenceErrors]);
        }

        question.Update(
            input.QuestionText,
            input.QuestionType,
            input.ExplanationTh,
            input.ExplanationEn,
            input.Points,
            input.SortOrder,
            input.WordId,
            input.GrammarRuleId,
            input.PronunciationId,
            input.IsActive,
            timeProvider.GetUtcNow());

        await questionRepository.SaveChangesAsync(cancellationToken);

        return Result<QuizQuestionDto>.Success(QuizQuestionReadModelBuilder.Map(question));
    }
}
