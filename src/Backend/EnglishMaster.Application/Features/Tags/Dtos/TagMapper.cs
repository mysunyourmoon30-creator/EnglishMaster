using EnglishMaster.Contracts.Tags;
using EnglishMaster.Domain.Tags;

namespace EnglishMaster.Application.Features.Tags.Dtos;

internal static class TagMapper
{
    public static TagDto ToDto(Tag tag)
    {
        return new TagDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.Description,
            tag.IsActive,
            tag.CreatedAt,
            tag.UpdatedAt);
    }
}
