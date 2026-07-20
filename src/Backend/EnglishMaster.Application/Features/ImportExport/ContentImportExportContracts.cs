using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ImportExport;

public interface IContentImportService
{
    Task<Result<ContentImportResult>> ImportWordsAsync(
        Stream stream,
        string fileName,
        string contentType,
        long fileSize,
        CancellationToken cancellationToken);
}

public interface IContentExportService
{
    Task<Result<ContentExportStream>> ExportWordsAsync(string? format, CancellationToken cancellationToken);

    Task<Result<ContentExportStream>> ExportGrammarTopicsAsync(string? format, CancellationToken cancellationToken);

    Task<Result<ContentExportStream>> ExportLessonsAsync(string? format, CancellationToken cancellationToken);

    Task<Result<ContentExportStream>> ExportCoursesAsync(string? format, CancellationToken cancellationToken);

    Task<Result<ContentExportStream>> ExportBooksAsync(string? format, CancellationToken cancellationToken);

    Task<Result<ContentExportStream>> ExportQuizzesAsync(string? format, CancellationToken cancellationToken);
}

public static class ContentImportExportLimits
{
    public const long MaximumImportFileSizeBytes = 1_048_576;
}

public sealed record ContentImportResult(
    int TotalRows,
    int ImportedCount,
    int FailedCount,
    IReadOnlyCollection<ContentImportError> Errors);

public sealed record ContentImportError(
    int RowNumber,
    string Field,
    string Message);

public sealed record ContentExportStream(
    string FileName,
    string ContentType,
    Func<Stream, CancellationToken, Task> WriteToAsync);
