namespace EnglishMaster.Contracts.Tags;

public sealed record TagSearchResponse(
    IReadOnlyCollection<TagDto> Items);
