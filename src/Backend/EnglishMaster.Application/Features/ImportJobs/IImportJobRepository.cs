using EnglishMaster.Application.Features.ImportJobs.Dtos;
using EnglishMaster.Domain.ImportJobs;

namespace EnglishMaster.Application.Features.ImportJobs;

public interface IImportJobRepository
{
    Task<ImportJobDto> AddAsync(ImportJob job, CancellationToken cancellationToken);
    Task<ImportJob?> GetEntityAsync(Guid id, CancellationToken cancellationToken);
    Task<ImportJobDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<ImportJobDto> SaveAsync(ImportJob job, CancellationToken cancellationToken);
    Task<ImportJobSearchResponse> SearchAsync(string? importType, string? format, string? status, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ImportJobRowDto>> GetRowsAsync(Guid importJobId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ImportValidationErrorDto>> GetErrorsAsync(Guid importJobId, CancellationToken cancellationToken);
}

public interface IImportParser
{
    IReadOnlyCollection<string> ParseRows(string format, string content);
}

public interface IImportValidationService
{
    Task ValidateAsync(ImportJob job, CancellationToken cancellationToken);
}

public interface IImportPreviewService
{
    Task ConfirmAsync(ImportJob job, CancellationToken cancellationToken);
}

public interface IImportRollbackService
{
    Task RollbackAsync(ImportJob job, CancellationToken cancellationToken);
}

public interface IImportRunService
{
    Task RunAsync(ImportJob job, CancellationToken cancellationToken);
}
