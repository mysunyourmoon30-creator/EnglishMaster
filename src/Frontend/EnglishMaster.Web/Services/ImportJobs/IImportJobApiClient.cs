using EnglishMaster.Contracts.ImportJobs;

namespace EnglishMaster.Web.Services.ImportJobs;

public interface IImportJobApiClient
{
    Task<ImportJobSearchResponse> SearchAsync(string? importType, string? format, string? status, CancellationToken cancellationToken);
    Task<ImportJobDto> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ImportJobRowDto>> GetRowsAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ImportValidationErrorDto>> GetErrorsAsync(Guid id, CancellationToken cancellationToken);
    Task<ImportJobDto> UploadAsync(UploadImportJobRequest request, CancellationToken cancellationToken);
    Task<ImportJobDto> ValidateAsync(Guid id, CancellationToken cancellationToken);
    Task<ImportJobDto> ConfirmAsync(Guid id, CancellationToken cancellationToken);
    Task<ImportJobDto> RunAsync(Guid id, CancellationToken cancellationToken);
    Task<ImportJobDto> CancelAsync(Guid id, CancellationToken cancellationToken);
    Task<ImportJobDto> RollbackAsync(Guid id, CancellationToken cancellationToken);
}
