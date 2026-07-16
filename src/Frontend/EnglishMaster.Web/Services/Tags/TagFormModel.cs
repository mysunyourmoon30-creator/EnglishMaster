using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Tags;

namespace EnglishMaster.Web.Services.Tags;

public sealed class TagFormModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public static TagFormModel FromDto(TagDto tag)
    {
        return new TagFormModel
        {
            Name = tag.Name,
            Description = tag.Description,
            IsActive = tag.IsActive
        };
    }

    public CreateTagRequest ToCreateRequest()
    {
        return new CreateTagRequest(Name, Description);
    }

    public UpdateTagRequest ToUpdateRequest()
    {
        return new UpdateTagRequest(Name, Description, IsActive);
    }
}
