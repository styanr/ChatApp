using ChatApp.Exceptions;
using ChatApp.Models.Files;
using ImageMagick;

namespace ChatApp.Services.Blobs;

public class ProfilePictureService : IProfilePictureService
{
    private readonly FileProcessor _fileProcessor;
    private readonly IBlobService _blobService;
    private readonly FileRestrictionsManager _fileRestrictionsManager;

    public ProfilePictureService([FromKeyedServices("images")] IBlobService blobService,
        FileRestrictionsManager fileRestrictionsManager, FileProcessor fileProcessor)
    {
        _fileProcessor = fileProcessor;
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

        await using var processedImage = _fileProcessor.ProcessProfilePicture(file.OpenReadStream());

        return await _blobService.UploadAsync(processedImage, contentType, "images", cancellationToken);
    }

    public async Task<FileResponse> DownloadProfilePictureAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _blobService.DownloadAsync(id, "images", cancellationToken);
    }
}