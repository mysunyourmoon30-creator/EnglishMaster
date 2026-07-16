namespace EnglishMaster.Application.Features.Tags.Commands;

public sealed record UpdateTagCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive);
