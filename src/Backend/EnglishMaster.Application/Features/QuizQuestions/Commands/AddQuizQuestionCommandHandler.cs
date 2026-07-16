using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Application.Features.QuizQuestions.Dtos;
using EnglishMaster.Application.Features.Quizzes;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public sealed class AddQuizQuestionCommandHandler
{
    private readonly IQuizQuestionRepository questionRepository;
    private readonly IQuizRepository quizRepository;
    private readonly IWordRepository wordRepository;
    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly IPronunciationRepository pronunciationRepository;
    private readonly TimeProvider timeProvider;

    public AddQuizQuestionCommandHandler(
        IQuizQuestionRepository questionRepository,
        IQuizRepository quizRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository,
        IPronunciationRepository pronunciationRepository,
        TimeProvider timeProvider)
    {
        this.questionRepository = questionRepository;
        this.quizRepository = quizRepository;
        this.wordRepository = wordRepository;
        this.grammarRuleRepository = grammarRuleRepository;
        this.pronunciationRepository = pronunciationRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<QuizQuestionDto>> HandleAsync(
        AddQuizQuestionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.QuizId == Guid.Empty)
        {
            return Result<QuizQuestionDto>.Validation(
                new ValidationError(nameof(command.QuizId), $"{nameof(command.QuizId)} cannot be empty."));
        }

        var quiz = await quizRepository.GetByIdAsync(command.QuizId, cancellationToken);
        if (quiz is null)
        {
            return Result<QuizQuestionDto>.NotFound(nameof(command.QuizId), "Quiz was not found.");
        }

        if (!quiz.IsActive)
        {
            return Result<QuizQuestionDto>.Validation(
                new ValidationError(nameof(command.QuizId), "Quiz is inactive."));
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
            isActive: true);
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

        var question = QuizQuestion.Create(
            command.QuizId,
            input.QuestionText,
            input.QuestionType,
            input.ExplanationTh,
            input.ExplanationEn,
            input.Points,
            input.SortOrder,
            input.WordId,
            input.GrammarRuleId,
            input.PronunciationId,
            timeProvider.GetUtcNow());

        await questionRepository.AddAsync(question, cancellationToken);
        await questionRepository.SaveChangesAsync(cancellationToken);

        return Result<QuizQuestionDto>.Success(QuizQuestionReadModelBuilder.Map(question));
    }
}
