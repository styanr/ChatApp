﻿using ChatApp.Exceptions;
using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public class FileService : IFileService
{
    private readonly IBlobService _blobService;
    private readonly FileRestrictionsManager _fileRestrictionsManager;
    private readonly FileProcessor _fileProcessor;

    public FileService(IBlobService blobService, FileRestrictionsManager fileRestrictionsManager, FileProcessor fileProcessor)
    {
        _blobService = blobService;
        _fileRestrictionsManager = fileRestrictionsManager;
        _fileProcessor = fileProcessor;
    }
    public async Task<Guid> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        ValidateFile(file);

        var isImage = _fileRestrictionsManager.IsContentTypeAllowed(file.ContentType, isImage: true);
        await using var fileStream = GetFileStream(file, isImage);

        return await _blobService.UploadAsync(fileStream, file.ContentType, "files", cancellationToken);
    }

    private void ValidateFile(IFormFile file)
    {
        if (!_fileRestrictionsManager.IsContentTypeAllowed(file.ContentType, isImage: false))
        {
            throw new InvalidFileException("Invalid file type.");
        }

        if (!_fileRestrictionsManager.IsFileSizeAllowed(file.Length))
        {
            throw new InvalidFileException("File size is too large.");
        }
    }

    private Stream GetFileStream(IFormFile file, bool isImage)
    {
        return isImage ? _fileProcessor.CompressImage(file.OpenReadStream(), 80) : file.OpenReadStream();
    }


    public async Task<FileResponse> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _blobService.DownloadAsync(id, "files", cancellationToken);
    }
}