using EnglishMaster.Application.Features.Certificates;
using EnglishMaster.Domain.Certificates;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Certificates;

public sealed class EfIssuedCertificateRepository : IIssuedCertificateRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfIssuedCertificateRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IssuedCertificateDto?> GetByUserAndCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken)
    {
        var certificate = await dbContext.IssuedCertificates.AsNoTracking()
            .SingleOrDefaultAsync(item => item.UserId == userId && item.CourseId == courseId, cancellationToken);

        return certificate is null ? null : ToDto(certificate);
    }

    public async Task<IReadOnlyCollection<IssuedCertificateDto>> GetByUserAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var certificates = await dbContext.IssuedCertificates.AsNoTracking()
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.IssuedAt)
            .Take(limit)
            .ToArrayAsync(cancellationToken);

        return certificates.Select(ToDto).ToArray();
    }

    public async Task<CertificateGenerationCourse?> GetCompletedCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken)
    {
        var course = await dbContext.CourseProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId && progress.CourseId == courseId && progress.Status == LearningProgressStatus.Completed && progress.ProgressPercent >= 100)
            .Join(dbContext.Courses.AsNoTracking().Where(course => course.IsActive && course.IsPublished),
                progress => progress.CourseId,
                course => course.Id,
                (_, course) => new CertificateGenerationCourse(course.Id, course.Title))
            .SingleOrDefaultAsync(cancellationToken);

        return course;
    }

    public async Task<CertificateGenerationUser?> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.AppUsers.AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new CertificateGenerationUser(user.Id, user.UserName, user.Email))
            .SingleOrDefaultAsync(cancellationToken);

        return user;
    }

    public async Task<CertificateTemplate?> GetActiveTemplateAsync(Guid? templateId, CancellationToken cancellationToken)
    {
        var query = dbContext.CertificateTemplates.AsNoTracking().Where(template => template.IsActive);
        if (templateId.HasValue)
        {
            query = query.Where(template => template.Id == templateId.Value);
        }

        return await query.OrderBy(template => template.CreatedAt).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IssuedCertificateDto> AddAsync(IssuedCertificate certificate, CancellationToken cancellationToken)
    {
        dbContext.IssuedCertificates.Add(certificate);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(certificate);
    }

    private static IssuedCertificateDto ToDto(IssuedCertificate certificate) =>
        new(certificate.Id, certificate.UserId, certificate.CourseId, certificate.TemplateId, certificate.VerificationCode, certificate.RecipientName, certificate.CourseTitle, certificate.TemplateCode, certificate.RenderedBody, certificate.IssuedAt, certificate.IsRevoked);
}
