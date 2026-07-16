using System.Security.Claims;
using System.Text;
using EnglishMaster.Application.Features.ImportJobs.Commands;
using EnglishMaster.Application.Features.ImportJobs.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.ImportJobs;
using EnglishMaster.Shared.Results;
using AppImportJobDto = EnglishMaster.Application.Features.ImportJobs.Dtos.ImportJobDto;
using AppImportJobRowDto = EnglishMaster.Application.Features.ImportJobs.Dtos.ImportJobRowDto;
using AppImportJobSearchResponse = EnglishMaster.Application.Features.ImportJobs.Dtos.ImportJobSearchResponse;
using AppImportValidationErrorDto = EnglishMaster.Application.Features.ImportJobs.Dtos.ImportValidationErrorDto;

namespace EnglishMaster.Api.Endpoints;

public static class ImportJobEndpoints
{
    public static IEndpointRouteBuilder MapImportJobEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/import-jobs")
            .WithTags("Import Jobs");

        group.MapGet("", SearchAsync).RequireAuthorization(Permissions.ImportRead);
        group.MapGet("/{id:guid}", GetAsync).RequireAuthorization(Permissions.ImportRead);
        group.MapGet("/{id:guid}/rows", GetRowsAsync).RequireAuthorization(Permissions.ImportRead);
        group.MapGet("/{id:guid}/errors", GetErrorsAsync).RequireAuthorization(Permissions.ImportRead);
        group.MapGet("/{id:guid}/errors/export", ExportErrorsAsync).RequireAuthorization(Permissions.ImportRead);
        group.MapPost("/upload", UploadAsync).RequireAuthorization(Permissions.ImportUpload);
        group.MapPost("/{id:guid}/validate", ValidateAsync).RequireAuthorization(Permissions.ImportValidate);
        group.MapPost("/{id:guid}/confirm", ConfirmAsync).RequireAuthorization(Permissions.ImportRun);
        group.MapPost("/{id:guid}/run", RunAsync).RequireAuthorization(Permissions.ImportRun);
        group.MapPost("/{id:guid}/cancel", CancelAsync).RequireAuthorization(Permissions.ImportRun);
        group.MapPost("/{id:guid}/rollback", RollbackAsync).RequireAuthorization(Permissions.ImportRollback);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(ImportJobQueryHandler handler, string? importType, string? format, string? status, int? pageNumber, int? pageSize, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchImportJobsQuery(importType, format, status, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetAsync(Guid id, ImportJobQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetAsync(new GetImportJobByIdQuery(id), cancellationToken));

    private static async Task<IResult> GetRowsAsync(Guid id, ImportJobQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetRowsAsync(new GetImportJobRowsQuery(id), cancellationToken));

    private static async Task<IResult> GetErrorsAsync(Guid id, ImportJobQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetErrorsAsync(new GetImportValidationErrorsQuery(id), cancellationToken));

    private static async Task<IResult> ExportErrorsAsync(Guid id, ImportJobQueryHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.GetErrorsAsync(new GetImportValidationErrorsQuery(id), cancellationToken);
        if (result.Status != ResultStatus.Success)
        {
            return Results.Problem();
        }

        var builder = new StringBuilder("RowId,FieldName,ErrorCode,Severity,ErrorMessage\r\n");
        foreach (var error in result.Value!)
        {
            builder.AppendLine($"{error.ImportJobRowId},{Escape(error.FieldName)},{Escape(error.ErrorCode)},{error.Severity},{Escape(error.ErrorMessage)}");
        }

        return Results.Text(builder.ToString(), "text/csv");
    }

    private static async Task<IResult> UploadAsync(UploadImportJobRequest request, ClaimsPrincipal user, ImportJobCommandHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.UploadAsync(new UploadImportFileCommand(request.ImportType, request.Format, request.OriginalFileName, request.Content, user.Identity?.Name ?? "Unknown"), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/import-jobs/{result.Value!.Id}", ToContract(result.Value))
            : ToHttpResult(result);
    }

    private static async Task<IResult> ValidateAsync(Guid id, ImportJobCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.ValidateAsync(new ValidateImportJobCommand(id), cancellationToken));

    private static async Task<IResult> ConfirmAsync(Guid id, ImportJobCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.ConfirmAsync(new ConfirmImportJobCommand(id), cancellationToken));

    private static async Task<IResult> RunAsync(Guid id, ImportJobCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.RunAsync(new RunImportJobCommand(id), cancellationToken));

    private static async Task<IResult> CancelAsync(Guid id, ImportJobCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.CancelAsync(new CancelImportJobCommand(id), cancellationToken));

    private static async Task<IResult> RollbackAsync(Guid id, ImportJobCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.RollbackAsync(new RollbackImportJobCommand(id), cancellationToken));

    private static IResult ToHttpResult(Result<AppImportJobDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };

    private static IResult ToHttpResult(Result<AppImportJobSearchResponse> result) =>
        result.Status == ResultStatus.Success
            ? Results.Ok(new ImportJobSearchResponse(result.Value!.Items.Select(ToContract).ToArray(), result.Value.PageNumber, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages, result.Value.HasPreviousPage, result.Value.HasNextPage))
            : Results.Problem();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppImportJobRowDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppImportValidationErrorDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static ImportJobDto ToContract(AppImportJobDto job) =>
        new(job.Id, job.ImportType, job.Format, job.Status, job.FileName, job.OriginalFileName, job.FileSize, job.RequestedBy, job.RequestedAt, job.ValidatedAt, job.ConfirmedAt, job.CompletedAt, job.FailedAt, job.RolledBackAt, job.TotalRows, job.ValidRows, job.InvalidRows, job.ImportedRows, job.FailedRows, job.ErrorMessage, job.CreatedAt, job.UpdatedAt);

    private static ImportJobRowDto ToContract(AppImportJobRowDto row) =>
        new(row.Id, row.ImportJobId, row.RowNumber, row.RawDataJson, row.ParsedDataJson, row.Status, row.ErrorMessage, row.CreatedEntityType, row.CreatedEntityId, row.UpdatedEntityType, row.UpdatedEntityId, row.CreatedAt, row.UpdatedAt);

    private static ImportValidationErrorDto ToContract(AppImportValidationErrorDto error) =>
        new(error.Id, error.ImportJobRowId, error.FieldName, error.ErrorCode, error.ErrorMessage, error.Severity, error.CreatedAt, error.UpdatedAt);

    private static string Escape(string value) => $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors) =>
        errors.GroupBy(error => error.Field).ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());
}
