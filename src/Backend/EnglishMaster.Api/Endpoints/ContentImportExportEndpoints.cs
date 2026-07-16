using EnglishMaster.Application.Features.ImportExport;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace EnglishMaster.Api.Endpoints;

public static class ContentImportExportEndpoints
{
    public static IEndpointRouteBuilder MapContentImportExportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var importGroup = endpoints.MapGroup("/api/v1/import")
            .WithTags("Import");
        importGroup.MapPost("/words", ImportWordsAsync)
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(ContentImportExportLimits.MaximumImportFileSizeBytes))
            .RequireAuthorization(Permissions.WordsCreate);

        var exportGroup = endpoints.MapGroup("/api/v1/export")
            .WithTags("Export");
        exportGroup.MapGet("/words", ExportWordsAsync).RequireAuthorization(Permissions.WordsRead);
        exportGroup.MapGet("/grammar-topics", ExportGrammarTopicsAsync).RequireAuthorization(Permissions.GrammarRead);
        exportGroup.MapGet("/lessons", ExportLessonsAsync).RequireAuthorization(Permissions.LessonsRead);
        exportGroup.MapGet("/courses", ExportCoursesAsync).RequireAuthorization(Permissions.CoursesRead);
        exportGroup.MapGet("/books", ExportBooksAsync).RequireAuthorization(Permissions.BooksRead);
        exportGroup.MapGet("/quizzes", ExportQuizzesAsync).RequireAuthorization(Permissions.QuizzesRead);

        return endpoints;
    }

    private static async Task<IResult> ImportWordsAsync(
        [FromForm] IFormFile? file,
        IContentImportService importService,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(file)] = ["File is required."]
            });
        }

        await using var stream = file.OpenReadStream();
        var result = await importService.ImportWordsAsync(
            stream,
            file.FileName,
            file.ContentType,
            file.Length,
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> ExportWordsAsync(
        IContentExportService exportService,
        string? format,
        CancellationToken cancellationToken)
    {
        var result = await exportService.ExportWordsAsync(format, cancellationToken);
        return ToFileResult(result);
    }

    private static async Task<IResult> ExportGrammarTopicsAsync(
        IContentExportService exportService,
        string? format,
        CancellationToken cancellationToken)
    {
        var result = await exportService.ExportGrammarTopicsAsync(format, cancellationToken);
        return ToFileResult(result);
    }

    private static async Task<IResult> ExportLessonsAsync(
        IContentExportService exportService,
        string? format,
        CancellationToken cancellationToken)
    {
        var result = await exportService.ExportLessonsAsync(format, cancellationToken);
        return ToFileResult(result);
    }

    private static async Task<IResult> ExportCoursesAsync(
        IContentExportService exportService,
        string? format,
        CancellationToken cancellationToken)
    {
        var result = await exportService.ExportCoursesAsync(format, cancellationToken);
        return ToFileResult(result);
    }

    private static async Task<IResult> ExportBooksAsync(
        IContentExportService exportService,
        string? format,
        CancellationToken cancellationToken)
    {
        var result = await exportService.ExportBooksAsync(format, cancellationToken);
        return ToFileResult(result);
    }

    private static async Task<IResult> ExportQuizzesAsync(
        IContentExportService exportService,
        string? format,
        CancellationToken cancellationToken)
    {
        var result = await exportService.ExportQuizzesAsync(format, cancellationToken);
        return ToFileResult(result);
    }

    private static IResult ToFileResult(Result<ContentExportResult> result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.File(
                result.Value!.Content,
                result.Value.ContentType,
                result.Value.FileName),
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
