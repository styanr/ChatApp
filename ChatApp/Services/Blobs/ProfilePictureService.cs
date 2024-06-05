using ChatApp.Exceptions;
using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public class ProfilePictureService : IProfilePictureService
{
    private readonly IBlobService _blobService;

    public ProfilePictureService([FromKeyedServices("images")] IBlobService blobService)
    {
        _blobService = blobService;
    }
    
    public async Task<Guid> UploadProfilePictureAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        if (contentType != "image/png" && contentType != "image/jpeg")
        {
            throw new InvalidFileException("Invalid file type");
        }
        
        if (stream.Length > 5 * 1024 * 1024)
        {
            throw new InvalidFileException("File is too large");
        }
        
        return await _blobService.UploadAsync(stream, contentType, "images", cancellationToken);
    }

    public async Task<FileResponse> DownloadProfilePictureAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _blobService.DownloadAsync(id, "images", cancellationToken);
    }
}