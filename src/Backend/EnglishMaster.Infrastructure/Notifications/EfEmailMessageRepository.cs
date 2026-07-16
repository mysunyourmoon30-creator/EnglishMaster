using EnglishMaster.Application.Features.EmailMessages;
using EnglishMaster.Application.Features.EmailMessages.Dtos;
using EnglishMaster.Domain.Notifications;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class EfEmailMessageRepository : IEmailMessageRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfEmailMessageRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<EmailMessageDto> AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        dbContext.EmailMessages.Add(emailMessage);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(emailMessage);
    }

    public async Task<EmailMessageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var email = await dbContext.EmailMessages.AsNoTracking()
            .SingleOrDefaultAsync(email => email.Id == id, cancellationToken);

        return email is null ? null : ToDto(email);
    }

    public async Task<IReadOnlyCollection<EmailMessageDto>> GetPendingAsync(int maxItems, CancellationToken cancellationToken)
    {
        var emails = await dbContext.EmailMessages.AsNoTracking()
            .Where(email => email.Status == EmailMessageStatus.Pending)
            .OrderBy(email => email.CreatedAt)
            .Take(maxItems)
            .ToArrayAsync(cancellationToken);

        return emails.Select(ToDto).ToArray();
    }

    public async Task<EmailMessageSearchResponse> SearchAsync(
        string? status,
        string? toEmail,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.EmailMessages.AsNoTracking();
        if (Enum.TryParse<EmailMessageStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(email => email.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(toEmail))
        {
            var term = toEmail.Trim();
            query = query.Where(email => email.ToEmail.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var emailMessages = await query
            .OrderByDescending(email => email.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var items = emailMessages.Select(ToDto).ToArray();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new EmailMessageSearchResponse(items, pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<EmailMessageDto?> MarkSentAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var email = await dbContext.EmailMessages.FindAsync([id], cancellationToken);
        if (email is null)
        {
            return null;
        }

        email.MarkSent(now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(email);
    }

    public async Task<EmailMessageDto?> MarkFailedAsync(Guid id, string errorMessage, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var email = await dbContext.EmailMessages.FindAsync([id], cancellationToken);
        if (email is null)
        {
            return null;
        }

        email.MarkFailed(errorMessage, now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(email);
    }

    private static EmailMessageDto ToDto(EmailMessage email) =>
        new(
            email.Id,
            email.ToEmail,
            email.ToName,
            email.Subject,
            email.Body,
            email.IsHtml,
            email.Status.ToString(),
            email.SentAt,
            email.FailedAt,
            email.ErrorMessage,
            email.CreatedAt,
            email.UpdatedAt);
}
