using ChatApp.Exceptions;
using ChatApp.Models.Files;
using ChatApp.Services.Auth;
using ImageMagick;

namespace ChatApp.Services.Blobs;

public class ProfilePictureService : IProfilePictureService
{
    private readonly FileProcessor _fileProcessor;
    private readonly BlobOptions _blobOptions;
    private readonly IBlobService _blobService;
    private readonly FileRestrictionsManager _fileRestrictionsManager;

    public ProfilePictureService(IBlobService blobService,
        FileRestrictionsManager fileRestrictionsManager, FileProcessor fileProcessor, BlobOptions blobOptions)
    {
        _fileProcessor = fileProcessor;
        _blobOptions = blobOptions;
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

        return await _blobService.UploadAsync(processedImage, contentType, _blobOptions.ImageContainerName, cancellationToken);
    }

    public async Task<FileResponse> DownloadProfilePictureAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _blobService.DownloadAsync(id, _blobOptions.ImageContainerName, cancellationToken);
    }
}