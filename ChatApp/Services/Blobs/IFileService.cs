using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public interface IFileService
{
    Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default);
    Task<FileResponse> DownloadAsync(Guid id, CancellationToken cancellationToken = default);
}