using EnglishMaster.Contracts.Certificates;

namespace EnglishMaster.Web.Services.Certificates;

public interface ICertificateVerificationApiClient
{
    Task<PublicCertificateVerificationDto?> VerifyAsync(string verificationCode, CancellationToken cancellationToken);
}
