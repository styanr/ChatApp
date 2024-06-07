using ChatApp.Exceptions;
using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public class FileService : IFileService
{
    private readonly IBlobService _blobService;

    public FileService(IBlobService blobService)
    {
        _blobService = blobService;
    }
    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        if (contentType != "image/png" && contentType != "image/jpeg")
        {
            throw new InvalidFileException("Invalid file type");
        }
        
        if (stream.Length > 5 * 1024 * 1024)
        {
            throw new InvalidFileException("File is too large");
        }
        
        return await _blobService.UploadAsync(stream, contentType, "files", cancellationToken);
    }

    public async Task<FileResponse> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _blobService.DownloadAsync(id, "files", cancellationToken);
    }
}