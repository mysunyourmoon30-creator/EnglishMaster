using EnglishMaster.Application.Features.Courses.Commands;
using EnglishMaster.Application.Features.Courses.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var courses = endpoints.MapGroup("/api/v1/courses")
            .WithTags("Courses");

        courses.MapGet("", SearchCoursesAsync).RequireAuthorization(Permissions.CoursesRead);
        courses.MapGet("/{id:guid}", GetCourseAsync).RequireAuthorization(Permissions.CoursesRead);
        courses.MapPost("", CreateCourseAsync).RequireAuthorization(Permissions.CoursesCreate);
        courses.MapPut("/{id:guid}", UpdateCourseAsync).RequireAuthorization(Permissions.CoursesUpdate);
        courses.MapDelete("/{id:guid}", DeleteCourseAsync).RequireAuthorization(Permissions.CoursesDelete);
        courses.MapPost("/{id:guid}/publish", PublishCourseAsync).RequireAuthorization(Permissions.CoursesUpdate);
        courses.MapPost("/{id:guid}/unpublish", UnpublishCourseAsync).RequireAuthorization(Permissions.CoursesUpdate);
        courses.MapGet("/{courseId:guid}/lessons", GetLessonsByCourseIdAsync).RequireAuthorization(Permissions.CoursesRead);
        courses.MapPost("/{courseId:guid}/lessons/{lessonId:guid}", AddLessonAsync).RequireAuthorization(Permissions.CoursesUpdate);
        courses.MapDelete("/{courseId:guid}/lessons/{lessonId:guid}", RemoveLessonAsync).RequireAuthorization(Permissions.CoursesUpdate);
        courses.MapPost("/{courseId:guid}/lessons/reorder", ReorderLessonsAsync).RequireAuthorization(Permissions.CoursesUpdate);

        return endpoints;
    }

    private static async Task<IResult> SearchCoursesAsync(
        SearchCoursesQueryHandler handler,
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
            new SearchCoursesQuery(
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

    private static async Task<IResult> GetCourseAsync(
        Guid id,
        GetCourseByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetCourseByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateCourseAsync(
        CreateCourseRequest request,
        CreateCourseCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateCourseCommand(
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
            ? Results.Created($"/api/v1/courses/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateCourseAsync(
        Guid id,
        UpdateCourseRequest request,
        UpdateCourseCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateCourseCommand(
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

    private static async Task<IResult> DeleteCourseAsync(
        Guid id,
        DeleteCourseCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteCourseCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> PublishCourseAsync(
        Guid id,
        PublishCourseCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new PublishCourseCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> UnpublishCourseAsync(
        Guid id,
        UnpublishCourseCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new UnpublishCourseCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetLessonsByCourseIdAsync(
        Guid courseId,
        GetCourseLessonsByCourseIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetCourseLessonsByCourseIdQuery(courseId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddLessonAsync(
        Guid courseId,
        Guid lessonId,
        AddLessonToCourseCommandHandler handler,
        CancellationToken cancellationToken,
        int sortOrder = 0,
        bool isRequired = true)
    {
        var result = await handler.HandleAsync(
            new AddLessonToCourseCommand(courseId, lessonId, sortOrder, isRequired),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> RemoveLessonAsync(
        Guid courseId,
        Guid lessonId,
        RemoveLessonFromCourseCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RemoveLessonFromCourseCommand(courseId, lessonId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> ReorderLessonsAsync(
        Guid courseId,
        ReorderCourseLessonsRequest request,
        ReorderCourseLessonsCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ReorderCourseLessonsCommand(courseId, request.OrderedCourseLessonIds),
            cancellationToken);

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
