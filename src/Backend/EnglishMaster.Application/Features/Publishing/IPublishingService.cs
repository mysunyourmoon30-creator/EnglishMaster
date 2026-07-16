using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Publishing;

public interface IPublishingService
{
    Task<Result<PublishJobDto>> RunAsync(Guid publishJobId, CancellationToken cancellationToken);
}
