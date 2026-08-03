using System.Security.Claims;
using EnglishMaster.Application.Features.Diagnostics;
using EnglishMaster.Contracts.Diagnostics;
using EnglishMaster.Shared.Results;
using AppDiagnosticQuizDto = EnglishMaster.Application.Features.Diagnostics.DiagnosticQuizDto;
using AppDiagnosticResultDto = EnglishMaster.Application.Features.Diagnostics.DiagnosticResultDto;
using ContractDiagnosticChoiceDto = EnglishMaster.Contracts.Diagnostics.DiagnosticChoiceDto;
using ContractDiagnosticQuestionDto = EnglishMaster.Contracts.Diagnostics.DiagnosticQuestionDto;
using ContractDiagnosticQuizDto = EnglishMaster.Contracts.Diagnostics.DiagnosticQuizDto;
using ContractDiagnosticResultDto = EnglishMaster.Contracts.Diagnostics.DiagnosticResultDto;

namespace EnglishMaster.Api.Endpoints;

public static class DiagnosticEndpoints
{
    public static IEndpointRouteBuilder MapDiagnosticEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/me/diagnostics")
            .WithTags("Diagnostics")
            .RequireAuthorization();
        group.MapGet("/quizzes/{quizId:guid}", GetQuizAsync);
        group.MapPost("/quizzes/{quizId:guid}/submit", SubmitAsync);
        return endpoints;
    }

    private static async Task<IResult> GetQuizAsync(
        ClaimsPrincipal user,
        DiagnosticHandler handler,
        Guid quizId,
        CancellationToken cancellationToken)
    {
        if (!TryUserId(user))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetQuizAsync(new GetDiagnosticQuizQuery(quizId), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Ok(ToContract(result.Value!))
            : Results.NotFound();
    }

    private static async Task<IResult> SubmitAsync(
        ClaimsPrincipal user,
        DiagnosticHandler handler,
        Guid quizId,
        SubmitDiagnosticRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryUserId(user))
        {
            return Results.Unauthorized();
        }

        var answers = request.Answers?.Select(answer => new DiagnosticAnswer(answer.QuestionId, answer.ChoiceId)).ToArray();
        var result = await handler.SubmitAsync(new SubmitDiagnosticCommand(quizId, answers), cancellationToken);
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.ValidationError => Results.ValidationProblem(
                result.Errors.GroupBy(error => error.Field).ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Message).ToArray())),
            _ => Results.NotFound()
        };
    }

    private static ContractDiagnosticQuizDto ToContract(AppDiagnosticQuizDto quiz) =>
        new(
            quiz.Id,
            quiz.Title,
            quiz.Summary,
            quiz.TimeLimitMinutes,
            quiz.PassingScore,
            quiz.CefrLevel,
            quiz.Questions.Select(question => new ContractDiagnosticQuestionDto(
                question.Id,
                question.Text,
                question.QuestionType,
                question.SortOrder,
                question.Choices.Select(choice =>
                    new ContractDiagnosticChoiceDto(choice.Id, choice.Text, choice.SortOrder)).ToArray())).ToArray());

    private static ContractDiagnosticResultDto ToContract(AppDiagnosticResultDto result) =>
        new(
            result.QuizId,
            result.QuizTitle,
            result.CorrectCount,
            result.TotalQuestions,
            result.PercentageScore,
            result.PassingScore,
            result.Passed,
            result.AssessedCefrLevel,
            result.Recommendation);

    private static bool TryUserId(ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) && userId != Guid.Empty;
}
