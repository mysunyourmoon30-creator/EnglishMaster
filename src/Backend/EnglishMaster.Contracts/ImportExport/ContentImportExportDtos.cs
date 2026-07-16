namespace EnglishMaster.Contracts.ImportExport;

public sealed record ContentImportResult(
    int TotalRows,
    int ImportedCount,
    int FailedCount,
    IReadOnlyCollection<ContentImportError> Errors);

public sealed record ContentImportError(
    int RowNumber,
    string Field,
    string Message);

public sealed record ContentExportResult(
    string FileName,
    string ContentType,
    byte[] Content);
