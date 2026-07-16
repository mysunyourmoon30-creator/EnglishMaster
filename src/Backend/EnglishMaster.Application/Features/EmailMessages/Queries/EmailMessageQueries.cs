using EnglishMaster.Application.Features.EmailMessages.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.EmailMessages.Queries;

public sealed record SearchEmailMessagesQuery(string? Status, string? ToEmail, int? PageNumber, int? PageSize);

public sealed class EmailMessageQueryHandler
{
    private readonly IEmailMessageRepository repository;

    public EmailMessageQueryHandler(IEmailMessageRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<EmailMessageSearchResponse>> SearchAsync(SearchEmailMessagesQuery query, CancellationToken cancellationToken) =>
        Result<EmailMessageSearchResponse>.Success(await repository.SearchAsync(
            query.Status,
            query.ToEmail,
            Math.Max(query.PageNumber ?? 1, 1),
            Math.Clamp(query.PageSize ?? 20, 1, 100),
            cancellationToken));
}
