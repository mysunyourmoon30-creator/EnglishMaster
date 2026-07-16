using EnglishMaster.Contracts.ImportExport;

namespace EnglishMaster.Web.Services.ImportExport;

public interface IImportExportApiClient
{
    Task<ContentImportResult> ImportWordsAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<ExportFileResult> ExportAsync(
        string entity,
        string format,
        CancellationToken cancellationToken);
}

public sealed record ExportFileResult(
    string FileName,
    string ContentType,
    byte[] Content);
