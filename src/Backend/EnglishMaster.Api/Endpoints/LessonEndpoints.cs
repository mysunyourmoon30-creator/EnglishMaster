using EnglishMaster.Application.Features.LessonSections.Commands;
using EnglishMaster.Application.Features.LessonSections.Queries;
using EnglishMaster.Application.Features.Lessons.Commands;
using EnglishMaster.Application.Features.Lessons.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class LessonEndpoints
{
    public static IEndpointRouteBuilder MapLessonEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var lessons = endpoints.MapGroup("/api/v1/lessons")
            .WithTags("Lessons");

        lessons.MapGet("", SearchLessonsAsync).RequireAuthorization(Permissions.LessonsRead);
        lessons.MapGet("/{id:guid}", GetLessonAsync).RequireAuthorization(Permissions.LessonsRead);
        lessons.MapPost("", CreateLessonAsync).RequireAuthorization(Permissions.LessonsCreate);
        lessons.MapPut("/{id:guid}", UpdateLessonAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessons.MapDelete("/{id:guid}", DeleteLessonAsync).RequireAuthorization(Permissions.LessonsDelete);
        lessons.MapPost("/{id:guid}/publish", PublishLessonAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessons.MapPost("/{id:guid}/unpublish", UnpublishLessonAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessons.MapGet("/{lessonId:guid}/sections", GetSectionsByLessonIdAsync).RequireAuthorization(Permissions.LessonsRead);
        lessons.MapPost("/{lessonId:guid}/sections", AddSectionAsync).RequireAuthorization(Permissions.LessonsCreate);
        lessons.MapPost("/{lessonId:guid}/sections/reorder", ReorderSectionsAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessons.MapPost("/{lessonId:guid}/words/{wordId:guid}", AddWordAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessons.MapDelete("/{lessonId:guid}/words/{wordId:guid}", RemoveWordAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessons.MapPost("/{lessonId:guid}/grammar-rules/{grammarRuleId:guid}", AddGrammarRuleAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessons.MapDelete("/{lessonId:guid}/grammar-rules/{grammarRuleId:guid}", RemoveGrammarRuleAsync).RequireAuthorization(Permissions.LessonsUpdate);

        var lessonSections = endpoints.MapGroup("/api/v1/lesson-sections")
            .WithTags("Lesson Sections");

        lessonSections.MapPut("/{id:guid}", UpdateSectionAsync).RequireAuthorization(Permissions.LessonsUpdate);
        lessonSections.MapDelete("/{id:guid}", DeleteSectionAsync).RequireAuthorization(Permissions.LessonsDelete);

        return endpoints;
    }

    private static async Task<IResult> SearchLessonsAsync(
        SearchLessonsQueryHandler handler,
        string? search,
        string? cefrLevel,
        Guid? categoryId,
        bool? isPublished,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchLessonsQuery(
                search,
                cefrLevel,
                categoryId,
                isPublished,
                isActive,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetLessonAsync(
        Guid id,
        GetLessonByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetLessonByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateLessonAsync(
        CreateLessonRequest request,
        CreateLessonCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateLessonCommand(
                request.Title,
                request.Summary,
                request.Description,
                request.CefrLevel,
                request.CategoryId,
                request.ThumbnailMediaId,
                request.EstimatedMinutes,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/lessons/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateLessonAsync(
        Guid id,
        UpdateLessonRequest request,
        UpdateLessonCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateLessonCommand(
                id,
                request.Title,
                request.Summary,
                request.Description,
                request.CefrLevel,
                request.CategoryId,
                request.ThumbnailMediaId,
                request.EstimatedMinutes,
                request.SortOrder,
                request.IsPublished,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteLessonAsync(
        Guid id,
        DeleteLessonCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteLessonCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> PublishLessonAsync(
        Guid id,
        PublishLessonCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new PublishLessonCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> UnpublishLessonAsync(
        Guid id,
        UnpublishLessonCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new UnpublishLessonCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddWordAsync(
        Guid lessonId,
        Guid wordId,
        AddWordToLessonCommandHandler handler,
        CancellationToken cancellationToken,
        int sortOrder = 0)
    {
        var result = await handler.HandleAsync(
            new AddWordToLessonCommand(lessonId, wordId, sortOrder),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> RemoveWordAsync(
        Guid lessonId,
        Guid wordId,
        RemoveWordFromLessonCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RemoveWordFromLessonCommand(lessonId, wordId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddGrammarRuleAsync(
        Guid lessonId,
        Guid grammarRuleId,
        AddGrammarRuleToLessonCommandHandler handler,
        CancellationToken cancellationToken,
        int sortOrder = 0)
    {
        var result = await handler.HandleAsync(
            new AddGrammarRuleToLessonCommand(lessonId, grammarRuleId, sortOrder),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> RemoveGrammarRuleAsync(
        Guid lessonId,
        Guid grammarRuleId,
        RemoveGrammarRuleFromLessonCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RemoveGrammarRuleFromLessonCommand(lessonId, grammarRuleId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetSectionsByLessonIdAsync(
        Guid lessonId,
        GetLessonSectionsByLessonIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetLessonSectionsByLessonIdQuery(lessonId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddSectionAsync(
        Guid lessonId,
        CreateLessonSectionRequest request,
        AddLessonSectionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AddLessonSectionCommand(
                lessonId,
                request.Title,
                request.ContentMarkdown,
                request.SectionType,
                request.MediaId,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/lesson-sections/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> ReorderSectionsAsync(
        Guid lessonId,
        ReorderLessonSectionsRequest request,
        ReorderLessonSectionsCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ReorderLessonSectionsCommand(lessonId, request.OrderedSectionIds),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> UpdateSectionAsync(
        Guid id,
        UpdateLessonSectionRequest request,
        UpdateLessonSectionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateLessonSectionCommand(
                id,
                request.Title,
                request.ContentMarkdown,
                request.SectionType,
                request.MediaId,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteSectionAsync(
        Guid id,
        DeleteLessonSectionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteLessonSectionCommand(id), cancellationToken);

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
