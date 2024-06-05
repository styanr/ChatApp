using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public interface IBlobService
{
    Task<Guid> UploadAsync(Stream stream, string contentType, string containerName, CancellationToken cancellationToken = default);
    Task<FileResponse> DownloadAsync(Guid id, string containerName, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, string containerName, CancellationToken cancellationToken = default);
}