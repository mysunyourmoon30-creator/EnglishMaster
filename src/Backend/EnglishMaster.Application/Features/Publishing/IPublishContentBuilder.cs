using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.Application.Features.Publishing;

public interface IPublishContentBuilder
{
    Task<PublishContent> BuildAsync(
        PublishSourceType sourceType,
        Guid sourceId,
        PublishFormat format,
        string title,
        CancellationToken cancellationToken);
}
