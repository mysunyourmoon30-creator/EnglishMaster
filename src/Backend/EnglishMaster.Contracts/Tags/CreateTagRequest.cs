namespace EnglishMaster.Contracts.Tags;

public sealed record CreateTagRequest(
    string Name,
    string? Description);
