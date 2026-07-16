using EnglishMaster.Application.Features.Tags.Dtos;
using EnglishMaster.Domain.Tags;

namespace EnglishMaster.Application.Features.Tags;

public interface ITagRepository
{
    Task AddAsync(Tag tag, CancellationToken cancellationToken);

    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Tag>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedTagId,
        CancellationToken cancellationToken);

    Task<TagSearchResult> SearchAsync(
        TagSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
