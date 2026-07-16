using EnglishMaster.Domain.Certificates;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Certificates;

public sealed record CertificateTemplateDto(Guid Id, string Code, string Name, string Description, string BodyTemplate, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record CertificateTemplateSearchResponse(IReadOnlyCollection<CertificateTemplateDto> Items, int PageNumber, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage);

public sealed record IssuedCertificateDto(Guid Id, Guid UserId, Guid CourseId, Guid TemplateId, string VerificationCode, string RecipientName, string CourseTitle, string TemplateCode, string RenderedBody, DateTimeOffset IssuedAt, bool IsRevoked);

public sealed record PublicCertificateVerificationDto(string VerificationCode, string RecipientName, string CourseTitle, string TemplateCode, DateTimeOffset IssuedAt, bool IsRevoked, bool IsValid);

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
public sealed record GenerateCourseCertificateCommand(Guid UserId, Guid CourseId, Guid? TemplateId);
public sealed record GetMyCertificatesQuery(Guid UserId, int? Limit);
public sealed record VerifyCertificateQuery(string VerificationCode);

public interface IIssuedCertificateRepository
{
    Task<IssuedCertificateDto?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<IssuedCertificateDto>> GetByUserAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<PublicCertificateVerificationDto?> GetByVerificationCodeAsync(string verificationCode, CancellationToken cancellationToken);
    Task<CertificateGenerationCourse?> GetCompletedCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken);
    Task<CertificateGenerationUser?> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<CertificateTemplate?> GetActiveTemplateAsync(Guid? templateId, CancellationToken cancellationToken);
    Task<IssuedCertificateDto> AddAsync(IssuedCertificate certificate, CancellationToken cancellationToken);
}

public sealed record CertificateGenerationCourse(Guid Id, string Title);

public sealed record CertificateGenerationUser(Guid Id, string DisplayName, string Email);

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

public sealed class CertificateGenerationCommandHandler
{
    private readonly IIssuedCertificateRepository repository;
    private readonly TimeProvider timeProvider;

    public CertificateGenerationCommandHandler(IIssuedCertificateRepository repository, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<IssuedCertificateDto>> GenerateAsync(GenerateCourseCertificateCommand command, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByUserAndCourseAsync(command.UserId, command.CourseId, cancellationToken);
        if (existing is not null)
        {
            return Result<IssuedCertificateDto>.Success(existing);
        }

        var course = await repository.GetCompletedCourseAsync(command.UserId, command.CourseId, cancellationToken);
        if (course is null)
        {
            return Result<IssuedCertificateDto>.Validation(new ValidationError(nameof(command.CourseId), "Course must be completed before a certificate can be issued."));
        }

        var user = await repository.GetUserAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result<IssuedCertificateDto>.NotFound(nameof(command.UserId), "User was not found.");
        }

        var template = await repository.GetActiveTemplateAsync(command.TemplateId, cancellationToken);
        if (template is null)
        {
            return Result<IssuedCertificateDto>.Validation(new ValidationError(nameof(command.TemplateId), "An active certificate template is required."));
        }

        var now = timeProvider.GetUtcNow();
        var renderedBody = RenderTemplate(template.BodyTemplate, user.DisplayName, course.Title, now);
        var certificate = IssuedCertificate.Create(command.UserId, command.CourseId, template.Id, CreateVerificationCode(), user.DisplayName, course.Title, template.Code, renderedBody, now);
        return Result<IssuedCertificateDto>.Success(await repository.AddAsync(certificate, cancellationToken));
    }

    private static string CreateVerificationCode() =>
        $"cert-{Guid.NewGuid():N}";

    private static string RenderTemplate(string bodyTemplate, string studentName, string courseTitle, DateTimeOffset issuedAt) =>
        bodyTemplate
            .Replace("{{student}}", studentName, StringComparison.OrdinalIgnoreCase)
            .Replace("{{studentName}}", studentName, StringComparison.OrdinalIgnoreCase)
            .Replace("{{course}}", courseTitle, StringComparison.OrdinalIgnoreCase)
            .Replace("{{courseTitle}}", courseTitle, StringComparison.OrdinalIgnoreCase)
            .Replace("{{issuedAt}}", issuedAt.ToString("yyyy-MM-dd"), StringComparison.OrdinalIgnoreCase);
}

public sealed class CertificateGenerationQueryHandler
{
    private readonly IIssuedCertificateRepository repository;

    public CertificateGenerationQueryHandler(IIssuedCertificateRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<IssuedCertificateDto>>> GetMineAsync(GetMyCertificatesQuery query, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(query.Limit ?? 50, 1, 100);
        return Result<IReadOnlyCollection<IssuedCertificateDto>>.Success(await repository.GetByUserAsync(query.UserId, limit, cancellationToken));
    }

    public async Task<Result<PublicCertificateVerificationDto>> VerifyAsync(VerifyCertificateQuery query, CancellationToken cancellationToken)
    {
        var verificationCode = query.VerificationCode.Trim();
        if (verificationCode.Length == 0)
        {
            return Result<PublicCertificateVerificationDto>.Validation(new ValidationError(nameof(query.VerificationCode), "Verification code is required."));
        }

        var certificate = await repository.GetByVerificationCodeAsync(verificationCode, cancellationToken);
        return certificate is null
            ? Result<PublicCertificateVerificationDto>.NotFound(nameof(query.VerificationCode), "Certificate was not found.")
            : Result<PublicCertificateVerificationDto>.Success(certificate);
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
