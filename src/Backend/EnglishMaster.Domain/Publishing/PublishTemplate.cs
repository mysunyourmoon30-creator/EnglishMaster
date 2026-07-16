using EnglishMaster.Domain.Common;

namespace EnglishMaster.Domain.Publishing;

public sealed class PublishTemplate
{
    private PublishTemplate()
    {
        Name = string.Empty;
        Slug = string.Empty;
        Description = string.Empty;
        TemplateContent = string.Empty;
    }

    private PublishTemplate(
        Guid id,
        string? name,
        string? description,
        PublishFormat format,
        string? templateContent,
        bool isDefault,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = PublishingDomainGuard.RequiredId(id, nameof(id));
        CreatedAt = createdAt;
        Apply(name, description, format, templateContent, isDefault, isActive, updatedAt);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public PublishFormat Format { get; private set; }

    public string TemplateContent { get; private set; } = string.Empty;

    public bool IsDefault { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static PublishTemplate Create(
        string? name,
        string? description,
        PublishFormat format,
        string? templateContent,
        bool isDefault,
        DateTimeOffset now)
    {
        return new PublishTemplate(Guid.NewGuid(), name, description, format, templateContent, isDefault, true, now, now);
    }

    public void Update(
        string? name,
        string? description,
        PublishFormat format,
        string? templateContent,
        bool isDefault,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(name, description, format, templateContent, isDefault, isActive, now);
    }

    public void Activate(DateTimeOffset now)
    {
        IsActive = true;
        UpdatedAt = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }

    public static string GenerateSlug(string? name)
    {
        return SlugGenerator.Generate(name, nameof(Name), nameof(name), PublishingFieldLimits.TemplateName);
    }

    private void Apply(
        string? name,
        string? description,
        PublishFormat format,
        string? templateContent,
        bool isDefault,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Name = PublishingDomainGuard.RequiredText(name, nameof(Name), nameof(name), PublishingFieldLimits.TemplateName);
        Slug = GenerateSlug(Name);
        Description = PublishingDomainGuard.OptionalText(description, nameof(Description), nameof(description), PublishingFieldLimits.TemplateDescription);
        Format = format;
        TemplateContent = PublishingDomainGuard.OptionalText(templateContent, nameof(TemplateContent), nameof(templateContent), PublishingFieldLimits.TemplateContent);
        IsDefault = isDefault;
        IsActive = isActive;
        UpdatedAt = updatedAt;
    }
}
