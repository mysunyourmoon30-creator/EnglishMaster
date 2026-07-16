namespace EnglishMaster.Contracts.Certificates;

public sealed record CertificateTemplateDto(Guid Id, string Code, string Name, string Description, string BodyTemplate, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record CertificateTemplateSearchResponse(IReadOnlyCollection<CertificateTemplateDto> Items, int PageNumber, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage);

public sealed record CreateCertificateTemplateRequest(string Code, string Name, string Description, string BodyTemplate);

public sealed record UpdateCertificateTemplateRequest(string Name, string Description, string BodyTemplate, bool IsActive);

public sealed record IssuedCertificateDto(Guid Id, Guid CourseId, string VerificationCode, string RecipientName, string CourseTitle, string TemplateCode, string RenderedBody, DateTimeOffset IssuedAt, bool IsRevoked);

public sealed record GenerateCourseCertificateRequest(Guid? TemplateId);

public sealed record PublicCertificateVerificationDto(string VerificationCode, string RecipientName, string CourseTitle, string TemplateCode, DateTimeOffset IssuedAt, bool IsRevoked, bool IsValid);
