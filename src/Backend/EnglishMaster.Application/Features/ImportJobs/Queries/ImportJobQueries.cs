using EnglishMaster.Application.Features.ImportJobs.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.ImportJobs.Queries;

public sealed record GetImportJobByIdQuery(Guid Id);
public sealed record SearchImportJobsQuery(string? ImportType, string? Format, string? Status, int? PageNumber, int? PageSize);
public sealed record GetImportJobRowsQuery(Guid Id);
public sealed record GetImportValidationErrorsQuery(Guid Id);

public sealed class ImportJobQueryHandler
{
    private readonly IImportJobRepository repository;

    public ImportJobQueryHandler(IImportJobRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<ImportJobDto>> GetAsync(GetImportJobByIdQuery query, CancellationToken cancellationToken)
    {
        var job = await repository.GetAsync(query.Id, cancellationToken);
        return job is null ? Result<ImportJobDto>.NotFound(nameof(query.Id), "Import job was not found.") : Result<ImportJobDto>.Success(job);
    }

    public async Task<Result<ImportJobSearchResponse>> SearchAsync(SearchImportJobsQuery query, CancellationToken cancellationToken) =>
        Result<ImportJobSearchResponse>.Success(await repository.SearchAsync(query.ImportType, query.Format, query.Status, Math.Max(query.PageNumber ?? 1, 1), Math.Clamp(query.PageSize ?? 20, 1, 100), cancellationToken));

    public async Task<Result<IReadOnlyCollection<ImportJobRowDto>>> GetRowsAsync(GetImportJobRowsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ImportJobRowDto>>.Success(await repository.GetRowsAsync(query.Id, cancellationToken));

    public async Task<Result<IReadOnlyCollection<ImportValidationErrorDto>>> GetErrorsAsync(GetImportValidationErrorsQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ImportValidationErrorDto>>.Success(await repository.GetErrorsAsync(query.Id, cancellationToken));
}
