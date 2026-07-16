using EnglishMaster.Application.Features.QuizChoices.Commands;
using EnglishMaster.Application.Features.QuizChoices.Queries;
using EnglishMaster.Application.Features.QuizQuestions.Commands;
using EnglishMaster.Application.Features.QuizQuestions.Queries;
using EnglishMaster.Application.Features.Quizzes.Commands;
using EnglishMaster.Application.Features.Quizzes.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class QuizEndpoints
{
    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var quizzes = endpoints.MapGroup("/api/v1/quizzes")
            .WithTags("Quizzes");

        quizzes.MapGet("", SearchQuizzesAsync).RequireAuthorization(Permissions.QuizzesRead);
        quizzes.MapGet("/{id:guid}", GetQuizAsync).RequireAuthorization(Permissions.QuizzesRead);
        quizzes.MapPost("", CreateQuizAsync).RequireAuthorization(Permissions.QuizzesCreate);
        quizzes.MapPut("/{id:guid}", UpdateQuizAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        quizzes.MapDelete("/{id:guid}", DeleteQuizAsync).RequireAuthorization(Permissions.QuizzesDelete);
        quizzes.MapPost("/{id:guid}/publish", PublishQuizAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        quizzes.MapPost("/{id:guid}/unpublish", UnpublishQuizAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        quizzes.MapPost("/{id:guid}/activate", ActivateQuizAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        quizzes.MapPost("/{id:guid}/deactivate", DeactivateQuizAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        quizzes.MapGet("/{quizId:guid}/questions", GetQuestionsByQuizIdAsync).RequireAuthorization(Permissions.QuizzesRead);
        quizzes.MapPost("/{quizId:guid}/questions", AddQuestionAsync).RequireAuthorization(Permissions.QuizzesCreate);
        quizzes.MapPost("/{quizId:guid}/questions/reorder", ReorderQuestionsAsync).RequireAuthorization(Permissions.QuizzesUpdate);

        var questions = endpoints.MapGroup("/api/v1/quiz-questions")
            .WithTags("Quiz Questions");

        questions.MapGet("/{id:guid}", GetQuestionAsync).RequireAuthorization(Permissions.QuizzesRead);
        questions.MapPut("/{id:guid}", UpdateQuestionAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        questions.MapDelete("/{id:guid}", DeleteQuestionAsync).RequireAuthorization(Permissions.QuizzesDelete);
        questions.MapPost("/{id:guid}/activate", ActivateQuestionAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        questions.MapPost("/{id:guid}/deactivate", DeactivateQuestionAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        questions.MapGet("/{questionId:guid}/choices", GetChoicesByQuestionIdAsync).RequireAuthorization(Permissions.QuizzesRead);
        questions.MapPost("/{questionId:guid}/choices", AddChoiceAsync).RequireAuthorization(Permissions.QuizzesCreate);
        questions.MapPost("/{questionId:guid}/choices/reorder", ReorderChoicesAsync).RequireAuthorization(Permissions.QuizzesUpdate);

        var choices = endpoints.MapGroup("/api/v1/quiz-choices")
            .WithTags("Quiz Choices");

        choices.MapGet("/{id:guid}", GetChoiceAsync).RequireAuthorization(Permissions.QuizzesRead);
        choices.MapPut("/{id:guid}", UpdateChoiceAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        choices.MapDelete("/{id:guid}", DeleteChoiceAsync).RequireAuthorization(Permissions.QuizzesDelete);
        choices.MapPost("/{id:guid}/activate", ActivateChoiceAsync).RequireAuthorization(Permissions.QuizzesUpdate);
        choices.MapPost("/{id:guid}/deactivate", DeactivateChoiceAsync).RequireAuthorization(Permissions.QuizzesUpdate);

        return endpoints;
    }

    private static async Task<IResult> SearchQuizzesAsync(
        SearchQuizzesQueryHandler handler,
        string? search,
        string? cefrLevel,
        Guid? categoryId,
        Guid? lessonId,
        Guid? courseId,
        Guid? bookId,
        bool? isPublished,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchQuizzesQuery(
                search,
                cefrLevel,
                categoryId,
                lessonId,
                courseId,
                bookId,
                isPublished,
                isActive,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetQuizAsync(
        Guid id,
        GetQuizByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetQuizByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateQuizAsync(
        CreateQuizRequest request,
        CreateQuizCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateQuizCommand(
                request.Title,
                request.Summary,
                request.Description,
                request.CefrLevel,
                request.CategoryId,
                request.LessonId,
                request.CourseId,
                request.BookId,
                request.TimeLimitMinutes,
                request.PassingScore,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/quizzes/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateQuizAsync(
        Guid id,
        UpdateQuizRequest request,
        UpdateQuizCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateQuizCommand(
                id,
                request.Title,
                request.Summary,
                request.Description,
                request.CefrLevel,
                request.CategoryId,
                request.LessonId,
                request.CourseId,
                request.BookId,
                request.TimeLimitMinutes,
                request.PassingScore,
                request.SortOrder,
                request.IsPublished,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteQuizAsync(
        Guid id,
        DeleteQuizCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteQuizCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> PublishQuizAsync(
        Guid id,
        PublishQuizCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new PublishQuizCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> UnpublishQuizAsync(
        Guid id,
        UnpublishQuizCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new UnpublishQuizCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> ActivateQuizAsync(
        Guid id,
        ActivateQuizCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ActivateQuizCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> DeactivateQuizAsync(
        Guid id,
        DeactivateQuizCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeactivateQuizCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> GetQuestionsByQuizIdAsync(
        Guid quizId,
        GetQuizQuestionsByQuizIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetQuizQuestionsByQuizIdQuery(quizId), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> AddQuestionAsync(
        Guid quizId,
        CreateQuizQuestionRequest request,
        AddQuizQuestionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AddQuizQuestionCommand(
                quizId,
                request.QuestionText,
                request.QuestionType,
                request.ExplanationTh,
                request.ExplanationEn,
                request.Points,
                request.SortOrder,
                request.WordId,
                request.GrammarRuleId,
                request.PronunciationId),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/quiz-questions/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> ReorderQuestionsAsync(
        Guid quizId,
        ReorderQuizQuestionsRequest request,
        ReorderQuizQuestionsCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ReorderQuizQuestionsCommand(quizId, request.OrderedQuestionIds),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetQuestionAsync(
        Guid id,
        GetQuizQuestionByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetQuizQuestionByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> UpdateQuestionAsync(
        Guid id,
        UpdateQuizQuestionRequest request,
        UpdateQuizQuestionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateQuizQuestionCommand(
                id,
                request.QuestionText,
                request.QuestionType,
                request.ExplanationTh,
                request.ExplanationEn,
                request.Points,
                request.SortOrder,
                request.WordId,
                request.GrammarRuleId,
                request.PronunciationId,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteQuestionAsync(
        Guid id,
        DeleteQuizQuestionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteQuizQuestionCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> ActivateQuestionAsync(
        Guid id,
        ActivateQuizQuestionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ActivateQuizQuestionCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> DeactivateQuestionAsync(
        Guid id,
        DeactivateQuizQuestionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeactivateQuizQuestionCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> GetChoicesByQuestionIdAsync(
        Guid questionId,
        GetQuizChoicesByQuestionIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetQuizChoicesByQuestionIdQuery(questionId), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> AddChoiceAsync(
        Guid questionId,
        CreateQuizChoiceRequest request,
        AddQuizChoiceCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AddQuizChoiceCommand(
                questionId,
                request.ChoiceText,
                request.IsCorrect,
                request.ExplanationTh,
                request.ExplanationEn,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/quiz-choices/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> ReorderChoicesAsync(
        Guid questionId,
        ReorderQuizChoicesRequest request,
        ReorderQuizChoicesCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ReorderQuizChoicesCommand(questionId, request.OrderedChoiceIds),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetChoiceAsync(
        Guid id,
        GetQuizChoiceByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetQuizChoiceByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> UpdateChoiceAsync(
        Guid id,
        UpdateQuizChoiceRequest request,
        UpdateQuizChoiceCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateQuizChoiceCommand(
                id,
                request.ChoiceText,
                request.IsCorrect,
                request.ExplanationTh,
                request.ExplanationEn,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteChoiceAsync(
        Guid id,
        DeleteQuizChoiceCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteQuizChoiceCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> ActivateChoiceAsync(
        Guid id,
        ActivateQuizChoiceCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ActivateQuizChoiceCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> DeactivateChoiceAsync(
        Guid id,
        DeactivateQuizChoiceCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeactivateQuizChoiceCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static IResult ToHttpResult(Result result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.NoContent(),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(result.Value),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static Dictionary<string, string[]> ToValidationDictionary(
        IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());
    }
}
