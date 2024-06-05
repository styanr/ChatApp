using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public interface IProfilePictureService
{
    Task<Guid> UploadProfilePictureAsync(Stream stream, string contentType, CancellationToken cancellationToken = default);
    Task<FileResponse> DownloadProfilePictureAsync(Guid id, CancellationToken cancellationToken = default);
}