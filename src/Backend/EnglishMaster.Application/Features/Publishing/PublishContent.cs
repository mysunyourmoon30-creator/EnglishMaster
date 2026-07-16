namespace EnglishMaster.Application.Features.Publishing;

public sealed record PublishContent(
    string FileName,
    string Content,
    string ContentType);
