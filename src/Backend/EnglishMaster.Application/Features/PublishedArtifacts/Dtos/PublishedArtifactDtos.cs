using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.Application.Features.PublishedArtifacts.Dtos;

public sealed record PublishedArtifactSearchCriteria(
    Guid? PublishJobId,
    PublishSourceType? SourceType,
    Guid? SourceId,
    PublishFormat? Format,
    int PageNumber,
    int PageSize);

public sealed record PublishedArtifactSearchResult(
    IReadOnlyCollection<PublishedArtifact> Items,
    int TotalCount);

internal static class PublishedArtifactMapper
{
    public static PublishedArtifactDto ToDto(PublishedArtifact artifact)
    {
        return new PublishedArtifactDto(
            artifact.Id,
            artifact.PublishJobId,
            artifact.SourceType.ToString(),
            artifact.SourceId,
            artifact.Format.ToString(),
            artifact.FileName,
            artifact.FilePath,
            artifact.PublicUrl,
            artifact.FileSize,
            artifact.ContentType,
            artifact.CreatedAt,
            artifact.UpdatedAt);
    }
}
