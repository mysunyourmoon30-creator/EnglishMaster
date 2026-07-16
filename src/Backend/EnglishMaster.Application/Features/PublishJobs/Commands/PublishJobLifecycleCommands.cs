namespace EnglishMaster.Application.Features.PublishJobs.Commands;

public sealed record StartPublishJobCommand(Guid Id);

public sealed record CompletePublishJobCommand(Guid Id, string? OutputFileName, string? OutputPath);

public sealed record FailPublishJobCommand(Guid Id, string? ErrorMessage);

public sealed record CancelPublishJobCommand(Guid Id);

public sealed record RunPublishJobCommand(Guid Id);
