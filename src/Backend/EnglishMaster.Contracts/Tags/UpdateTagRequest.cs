namespace EnglishMaster.Contracts.Tags;

public sealed record UpdateTagRequest(
    string Name,
    string? Description,
    bool IsActive);
