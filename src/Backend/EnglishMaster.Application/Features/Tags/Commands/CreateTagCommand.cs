namespace EnglishMaster.Application.Features.Tags.Commands;

public sealed record CreateTagCommand(
    string Name,
    string? Description);
