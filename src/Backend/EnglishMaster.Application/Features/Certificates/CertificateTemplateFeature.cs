using EnglishMaster.Domain.Certificates;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Certificates;

public sealed record CertificateTemplateDto(Guid Id, string Code, string Name, string Description, string BodyTemplate, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record CertificateTemplateSearchResponse(IReadOnlyCollection<CertificateTemplateDto> Items, int PageNumber, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage);

public interface ICertificateTemplateRepository
{
    Task<CertificateTemplateDto> AddAsync(CertificateTemplate template, CancellationToken cancellationToken);
    Task<CertificateTemplate?> GetEntityByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CertificateTemplateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, Guid? excludedId, CancellationToken cancellationToken);
    Task<CertificateTemplateSearchResponse> SearchAsync(string? search, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public sealed record CreateCertificateTemplateCommand(string Code, string Name, string? Description, string BodyTemplate);
public sealed record UpdateCertificateTemplateCommand(Guid Id, string Name, string? Description, string BodyTemplate, bool IsActive);
public sealed record SetCertificateTemplateActiveCommand(Guid Id, bool IsActive);
public sealed record GetCertificateTemplateByIdQuery(Guid Id);
public sealed record SearchCertificateTemplatesQuery(string? Search, bool? IsActive, int? PageNumber, int? PageSize);

public sealed class CertificateTemplateCommandHandler
{
    private readonly ICertificateTemplateRepository repository;
    private readonly TimeProvider timeProvider;

    public CertificateTemplateCommandHandler(ICertificateTemplateRepository repository, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<CertificateTemplateDto>> CreateAsync(CreateCertificateTemplateCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var code = NormalizeCode(command.Code);
            if (await repository.CodeExistsAsync(code, null, cancellationToken))
            {
                return Result<CertificateTemplateDto>.Validation(new ValidationError(nameof(command.Code), "Certificate template code already exists."));
            }

            var template = CertificateTemplate.Create(code, command.Name, command.Description, command.BodyTemplate, timeProvider.GetUtcNow());
            return Result<CertificateTemplateDto>.Success(await repository.AddAsync(template, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return Result<CertificateTemplateDto>.Validation(new ValidationError(exception.ParamName ?? "template", exception.Message));
        }
    }

    public async Task<Result<CertificateTemplateDto>> UpdateAsync(UpdateCertificateTemplateCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var template = await repository.GetEntityByIdAsync(command.Id, cancellationToken);
            if (template is null)
            {
                return Result<CertificateTemplateDto>.NotFound(nameof(command.Id), "Certificate template was not found.");
            }

            template.Update(command.Name, command.Description, command.BodyTemplate, command.IsActive, timeProvider.GetUtcNow());
            await repository.SaveChangesAsync(cancellationToken);
            return Result<CertificateTemplateDto>.Success((await repository.GetByIdAsync(template.Id, cancellationToken))!);
        }
        catch (ArgumentException exception)
        {
            return Result<CertificateTemplateDto>.Validation(new ValidationError(exception.ParamName ?? "template", exception.Message));
        }
    }

    public async Task<Result<CertificateTemplateDto>> SetActiveAsync(SetCertificateTemplateActiveCommand command, CancellationToken cancellationToken)
    {
        var template = await repository.GetEntityByIdAsync(command.Id, cancellationToken);
        if (template is null)
        {
            return Result<CertificateTemplateDto>.NotFound(nameof(command.Id), "Certificate template was not found.");
        }

        if (command.IsActive)
        {
            template.Activate(timeProvider.GetUtcNow());
        }
        else
        {
            template.Deactivate(timeProvider.GetUtcNow());
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result<CertificateTemplateDto>.Success((await repository.GetByIdAsync(template.Id, cancellationToken))!);
    }

    private static string NormalizeCode(string? code)
    {
        var normalized = code?.Trim().ToLowerInvariant() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        if (normalized.Length > 64)
        {
            throw new ArgumentException("Code must be 64 characters or fewer.", nameof(code));
        }

        return normalized;
    }
}

public sealed class CertificateTemplateQueryHandler
{
    private readonly ICertificateTemplateRepository repository;

    public CertificateTemplateQueryHandler(ICertificateTemplateRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<CertificateTemplateDto>> GetByIdAsync(GetCertificateTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdAsync(query.Id, cancellationToken);
        return template is null
            ? Result<CertificateTemplateDto>.NotFound(nameof(query.Id), "Certificate template was not found.")
            : Result<CertificateTemplateDto>.Success(template);
    }

    public async Task<Result<CertificateTemplateSearchResponse>> SearchAsync(SearchCertificateTemplatesQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(query.PageNumber ?? 1, 1);
        var pageSize = Math.Clamp(query.PageSize ?? 20, 1, 50);
        return Result<CertificateTemplateSearchResponse>.Success(await repository.SearchAsync(query.Search, query.IsActive, pageNumber, pageSize, cancellationToken));
    }
}
