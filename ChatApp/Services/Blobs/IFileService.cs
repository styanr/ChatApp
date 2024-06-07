using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public interface IFileService
{
    Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<FileResponse> DownloadAsync(Guid id, CancellationToken cancellationToken = default);
}