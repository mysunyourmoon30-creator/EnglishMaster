using EnglishMaster.Application.Features.EmailMessages.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.EmailMessages.Queries;

public sealed record GetEmailProviderStatusQuery;

public sealed class EmailProviderQueryHandler
{
    private readonly IEmailProviderStatusService statusService;

    public EmailProviderQueryHandler(IEmailProviderStatusService statusService)
    {
        this.statusService = statusService;
    }

    public Task<Result<EmailProviderStatusDto>> GetStatusAsync(GetEmailProviderStatusQuery query, CancellationToken cancellationToken) =>
        Task.FromResult(Result<EmailProviderStatusDto>.Success(statusService.GetStatus()));
}
