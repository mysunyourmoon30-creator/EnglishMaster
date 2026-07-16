namespace EnglishMaster.Domain.BulkOperations;

public sealed class BulkOperationItem
{
    private BulkOperationItem()
    {
        ErrorMessage = string.Empty;
    }

    private BulkOperationItem(Guid bulkOperationId, Guid contentId, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        BulkOperationId = bulkOperationId == Guid.Empty ? throw new ArgumentException("BulkOperationId is required.", nameof(bulkOperationId)) : bulkOperationId;
        ContentId = contentId == Guid.Empty ? throw new ArgumentException("ContentId is required.", nameof(contentId)) : contentId;
        Status = BulkOperationItemStatus.Pending;
        ErrorMessage = string.Empty;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid BulkOperationId { get; private set; }
    public Guid ContentId { get; private set; }
    public BulkOperationItemStatus Status { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static BulkOperationItem Create(Guid bulkOperationId, Guid contentId, DateTimeOffset now) =>
        new(bulkOperationId, contentId, now);

    public void Succeed(DateTimeOffset now)
    {
        Status = BulkOperationItemStatus.Success;
        ErrorMessage = string.Empty;
        UpdatedAt = now;
    }

    public void Fail(string errorMessage, DateTimeOffset now)
    {
        Status = BulkOperationItemStatus.Failed;
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "Bulk item failed." : errorMessage.Trim();
        UpdatedAt = now;
    }

    public void Skip(string? reason, DateTimeOffset now)
    {
        Status = BulkOperationItemStatus.Skipped;
        ErrorMessage = reason?.Trim() ?? string.Empty;
        UpdatedAt = now;
    }
}
