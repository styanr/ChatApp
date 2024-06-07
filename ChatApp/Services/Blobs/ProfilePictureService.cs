using ChatApp.Exceptions;
using ChatApp.Models.Files;
using ImageMagick;

namespace ChatApp.Services.Blobs;

public class ProfilePictureService : IProfilePictureService
{
    private readonly IBlobService _blobService;
    private readonly FileRestrictionsManager _fileRestrictionsManager;

    public ProfilePictureService([FromKeyedServices("images")] IBlobService blobService, FileRestrictionsManager fileRestrictionsManager)
    {
        _blobService = blobService;
        _fileRestrictionsManager = fileRestrictionsManager;
    }
    
    public async Task<Guid> UploadProfilePictureAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (!_fileRestrictionsManager.IsContentTypeAllowed(file.ContentType, isImage: true))
        {
            throw new InvalidFileException("Invalid file type.");
        }
        
        if (!_fileRestrictionsManager.IsFileSizeAllowed(file.Length))
        {
            throw new InvalidFileException("File size is too large.");
        }
        
        var contentType = file.ContentType;
        await using var stream = file.OpenReadStream();

        using MagickImage image = new MagickImage(stream);
        
        var side = Math.Min(image.Width, image.Height);
        image.Crop(side, side, Gravity.Center);
        
        image.Format = MagickFormat.Jpeg;
        image.Quality = 80;
        
        using var memoryStream = new MemoryStream();
        await image.WriteAsync(memoryStream);
        memoryStream.Position = 0;

        return await _blobService.UploadAsync(memoryStream, contentType, "images", cancellationToken);
    }

    public async Task<FileResponse> DownloadProfilePictureAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _blobService.DownloadAsync(id, "images", cancellationToken);
    }
}