using EnglishMaster.Application.Features.Quizzes;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Diagnostics;

public sealed record GetDiagnosticQuizQuery(Guid QuizId);
public sealed record SubmitDiagnosticCommand(Guid QuizId, IReadOnlyCollection<DiagnosticAnswer>? Answers);

public sealed class DiagnosticHandler
{
    private const int MaximumAnswers = 100;
    private readonly IQuizRepository quizRepository;

    public DiagnosticHandler(IQuizRepository quizRepository)
    {
        this.quizRepository = quizRepository;
    }

    public async Task<Result<DiagnosticQuizDto>> GetQuizAsync(GetDiagnosticQuizQuery query, CancellationToken cancellationToken)
    {
        if (query.QuizId == Guid.Empty)
        {
            return Result<DiagnosticQuizDto>.NotFound(nameof(query.QuizId), "Diagnostic quiz was not found.");
        }

        var quiz = await quizRepository.GetByIdAsync(query.QuizId, cancellationToken);
        var questions = quiz is null ? [] : GetAnswerableQuestions(quiz);
        if (!IsEligible(quiz) || questions.Count == 0)
        {
            return Result<DiagnosticQuizDto>.NotFound(nameof(query.QuizId), "Diagnostic quiz was not found.");
        }

        return Result<DiagnosticQuizDto>.Success(new DiagnosticQuizDto(
            quiz!.Id,
            quiz.Title,
            quiz.Summary,
            quiz.TimeLimitMinutes,
            quiz.PassingScore,
            quiz.CefrLevel?.ToString(),
            questions.Select(question => new DiagnosticQuestionDto(
                question.Id,
                question.QuestionText,
                question.QuestionType.ToString(),
                question.SortOrder,
                GetActiveChoices(question).Select(choice =>
                    new DiagnosticChoiceDto(choice.Id, choice.ChoiceText, choice.SortOrder)).ToArray())).ToArray()));
    }

    public async Task<Result<DiagnosticResultDto>> SubmitAsync(SubmitDiagnosticCommand command, CancellationToken cancellationToken)
    {
        if (command.QuizId == Guid.Empty)
        {
            return Result<DiagnosticResultDto>.NotFound(nameof(command.QuizId), "Diagnostic quiz was not found.");
        }

        if (command.Answers is null)
        {
            return Validation("Answers are required.");
        }

        if (command.Answers.Count > MaximumAnswers)
        {
            return Validation($"A diagnostic submission cannot contain more than {MaximumAnswers} answers.");
        }

        if (command.Answers.Any(answer => answer.QuestionId == Guid.Empty || answer.ChoiceId == Guid.Empty))
        {
            return Validation("Every answer requires a question and a selected choice.");
        }

        if (command.Answers.Select(answer => answer.QuestionId).Distinct().Count() != command.Answers.Count)
        {
            return Validation("Each question may be answered only once.");
        }

        var quiz = await quizRepository.GetByIdAsync(command.QuizId, cancellationToken);
        var questions = quiz is null ? [] : GetAnswerableQuestions(quiz);
        if (!IsEligible(quiz) || questions.Count == 0)
        {
            return Result<DiagnosticResultDto>.NotFound(nameof(command.QuizId), "Diagnostic quiz was not found.");
        }

        if (command.Answers.Count != questions.Count)
        {
            return Validation("Submit exactly one answer for every diagnostic question.");
        }

        var questionsById = questions.ToDictionary(question => question.Id);
        var correctCount = 0;
        foreach (var answer in command.Answers)
        {
            if (!questionsById.TryGetValue(answer.QuestionId, out var question))
            {
                return Validation("The submission contains an unknown or inactive question.");
            }

            var choice = GetActiveChoices(question).SingleOrDefault(item => item.Id == answer.ChoiceId);
            if (choice is null)
            {
                return Validation("A selected choice does not belong to its submitted question.");
            }

            if (choice.IsCorrect)
            {
                correctCount++;
            }
        }

        var percentage = (int)Math.Round(correctCount * 100d / questions.Count, MidpointRounding.AwayFromZero);
        var passed = percentage >= quiz!.PassingScore;
        var cefrLevel = quiz.CefrLevel?.ToString();
        return Result<DiagnosticResultDto>.Success(new DiagnosticResultDto(
            quiz.Id,
            quiz.Title,
            correctCount,
            questions.Count,
            percentage,
            quiz.PassingScore,
            passed,
            cefrLevel,
            BuildRecommendation(passed, cefrLevel)));
    }

    private static bool IsEligible(Quiz? quiz) => quiz is { IsActive: true, IsPublished: true };

    private static IReadOnlyList<QuizQuestion> GetAnswerableQuestions(Quiz quiz) =>
        quiz.Questions
            .Where(question => question.IsActive &&
                GetActiveChoices(question).Count >= 2 &&
                GetActiveChoices(question).Count(choice => choice.IsCorrect) == 1)
            .OrderBy(question => question.SortOrder)
            .ThenBy(question => question.Id)
            .ToArray();

    private static IReadOnlyList<QuizChoice> GetActiveChoices(QuizQuestion question) =>
        question.Choices
            .Where(choice => choice.IsActive)
            .OrderBy(choice => choice.SortOrder)
            .ThenBy(choice => choice.Id)
            .ToArray();

    private static Result<DiagnosticResultDto> Validation(string message) =>
        Result<DiagnosticResultDto>.Validation(new ValidationError("Answers", message));

    private static string BuildRecommendation(bool passed, string? cefrLevel)
    {
        var scope = string.IsNullOrWhiteSpace(cefrLevel) ? "this quiz" : $"the {cefrLevel} quiz";
        return passed
            ? $"You met the threshold for {scope}. Continue at this band and try the next challenge; this is not a calibrated placement result."
            : $"Review the foundations covered by {scope}, then try again; this is not a calibrated placement result.";
    }
}
