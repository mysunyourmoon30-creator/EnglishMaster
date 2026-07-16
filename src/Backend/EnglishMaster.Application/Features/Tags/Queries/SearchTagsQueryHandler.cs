using EnglishMaster.Application.Features.Tags.Dtos;
using EnglishMaster.Contracts.Tags;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Tags.Queries;

public sealed class SearchTagsQueryHandler
{
    private const int MaximumSearchLength = TagFieldLimits.Name;

    private readonly ITagRepository tagRepository;

    public SearchTagsQueryHandler(ITagRepository tagRepository)
    {
        this.tagRepository = tagRepository;
    }

    public async Task<Result<TagSearchResponse>> HandleAsync(
        SearchTagsQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);

        if (errors.Count > 0)
        {
            return Result<TagSearchResponse>.Validation([.. errors]);
        }

        var result = await tagRepository.SearchAsync(
            new TagSearchCriteria(search, query.IsActive ?? true),
            cancellationToken);

        return Result<TagSearchResponse>.Success(new TagSearchResponse(
            result.Items.Select(TagMapper.ToDto).ToArray()));
    }

    private static string? NormalizeSearch(
        string? value,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > MaximumSearchLength)
        {
            errors.Add(new ValidationError(
                nameof(SearchTagsQuery.Search),
                $"{nameof(SearchTagsQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
        }

        return normalized;
    }
}
