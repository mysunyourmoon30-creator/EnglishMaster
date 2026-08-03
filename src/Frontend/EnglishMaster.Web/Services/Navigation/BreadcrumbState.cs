namespace EnglishMaster.Web.Services.Navigation;

public sealed class BreadcrumbState
{
    public Guid? CurrentEntityId { get; private set; }

    public string? CurrentLabel { get; private set; }

    public event Action? Changed;

    public void SetCurrentLabel(Guid entityId, string? label)
    {
        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity ID cannot be empty.", nameof(entityId));
        }

        CurrentEntityId = entityId;
        CurrentLabel = label;
        Changed?.Invoke();
    }

    public void Clear(Guid entityId)
    {
        if (CurrentEntityId != entityId)
        {
            return;
        }

        CurrentEntityId = null;
        CurrentLabel = null;
        Changed?.Invoke();
    }
}