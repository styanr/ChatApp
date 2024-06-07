using ChatApp.Exceptions;
using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public class FileService : IFileService
{
    private readonly IBlobService _blobService;
    private readonly FileRestrictionsManager _fileRestrictionsManager;

    public FileService(IBlobService blobService, FileRestrictionsManager fileRestrictionsManager)
    {
        _blobService = blobService;
        _fileRestrictionsManager = fileRestrictionsManager;
    }
    public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (!_fileRestrictionsManager.IsContentTypeAllowed(file.ContentType, isImage: false))
        {
            throw new InvalidFileException("Invalid file type.");
        }
        
        if (!_fileRestrictionsManager.IsFileSizeAllowed(file.Length))
        {
            throw new InvalidFileException("File size is too large.");
        }
        
        var contentType = file.ContentType;
        await using var stream = file.OpenReadStream();
        
        
        
        return await _blobService.UploadAsync(stream, contentType, "files", cancellationToken);
    }

    public async Task<FileResponse> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _blobService.DownloadAsync(id, "files", cancellationToken);
    }
}