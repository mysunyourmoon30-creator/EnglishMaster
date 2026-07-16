namespace EnglishMaster.Domain.BulkOperations;

public sealed class BulkOperation
{
    private readonly List<BulkOperationItem> items = [];

    private BulkOperation()
    {
        ContentType = string.Empty;
        RequestedBy = string.Empty;
        ErrorMessage = string.Empty;
        Note = string.Empty;
        ExportFormat = string.Empty;
    }

    private BulkOperation(BulkOperationType operationType, string contentType, string? requestedBy, IReadOnlyCollection<Guid> contentIds, string? note, Guid? categoryId, IEnumerable<Guid>? tagIds, string? exportFormat, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        OperationType = operationType;
        ContentType = Required(contentType, nameof(ContentType), 64);
        Status = BulkOperationStatus.Pending;
        RequestedBy = Optional(requestedBy, nameof(RequestedBy), 256);
        RequestedAt = now;
        var distinctContentIds = contentIds.Distinct().ToArray();
        TotalItems = distinctContentIds.Length;
        Note = Optional(note, nameof(Note), 1000);
        CategoryId = categoryId == Guid.Empty ? throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId)) : categoryId;
        TagIds = string.Join(",", (tagIds ?? []).Where(id => id != Guid.Empty).Distinct());
        ExportFormat = Optional(exportFormat, nameof(ExportFormat), 32);
        CreatedAt = now;
        UpdatedAt = now;

        foreach (var contentId in distinctContentIds)
        {
            items.Add(BulkOperationItem.Create(Id, contentId, now));
        }
    }

    public Guid Id { get; private set; }
    public BulkOperationType OperationType { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public BulkOperationStatus Status { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int TotalItems { get; private set; }
    public int SucceededItems { get; private set; }
    public int FailedItems { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public Guid? CategoryId { get; private set; }
    public string TagIds { get; private set; } = string.Empty;
    public string ExportFormat { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<BulkOperationItem> Items => items.AsReadOnly();

    public static BulkOperation Create(BulkOperationType operationType, string contentType, string? requestedBy, IReadOnlyCollection<Guid> contentIds, string? note, Guid? categoryId, IEnumerable<Guid>? tagIds, string? exportFormat, DateTimeOffset now) =>
        new(operationType, contentType, requestedBy, contentIds, note, categoryId, tagIds, exportFormat, now);

    public void Start(DateTimeOffset now)
    {
        if (Status != BulkOperationStatus.Pending)
        {
            throw new InvalidOperationException("Only pending bulk operations can be run.");
        }

        Status = BulkOperationStatus.Running;
        StartedAt = now;
        UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now)
    {
        if (Status is BulkOperationStatus.Completed or BulkOperationStatus.Failed or BulkOperationStatus.PartiallyCompleted)
        {
            throw new InvalidOperationException("Completed bulk operations cannot be cancelled.");
        }

        Status = BulkOperationStatus.Cancelled;
        CompletedAt = now;
        UpdatedAt = now;
    }

    public void Finish(DateTimeOffset now)
    {
        SucceededItems = items.Count(item => item.Status == BulkOperationItemStatus.Success);
        FailedItems = items.Count(item => item.Status == BulkOperationItemStatus.Failed);
        Status = FailedItems == 0
            ? BulkOperationStatus.Completed
            : SucceededItems == 0 ? BulkOperationStatus.Failed : BulkOperationStatus.PartiallyCompleted;
        ErrorMessage = FailedItems == 0 ? string.Empty : $"{FailedItems} item(s) failed.";
        CompletedAt = now;
        UpdatedAt = now;
    }

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = Optional(value, fieldName, maxLength);
        return normalized.Length == 0 ? throw new ArgumentException($"{fieldName} is required.", fieldName) : normalized;
    }

    private static string Optional(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        return normalized.Length > maxLength ? throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName) : normalized;
    }
}
