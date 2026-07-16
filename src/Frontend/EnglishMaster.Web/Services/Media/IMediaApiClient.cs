using EnglishMaster.Contracts.Media;
using Microsoft.AspNetCore.Components.Forms;

namespace EnglishMaster.Web.Services.Media;

public interface IMediaApiClient
{
    Task<MediaSearchResponse> SearchAsync(MediaSearchRequest request, CancellationToken cancellationToken);

    Task<MediaDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<MediaDto> CreateAsync(CreateMediaRequest request, CancellationToken cancellationToken);

    Task<MediaDto> UploadAsync(
        IBrowserFile file,
        string? altText,
        string? description,
        CancellationToken cancellationToken);

    Task<MediaDto> UpdateAsync(Guid id, UpdateMediaRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
