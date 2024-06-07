using ChatApp.Services.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;
[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IProfilePictureService _profilePictureService;
    private readonly IFileService _fileService;

    public FilesController(IProfilePictureService profilePictureService, IFileService fileService)
    {
        _profilePictureService = profilePictureService;
        _fileService = fileService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var id = await _fileService.UploadAsync(stream, file.ContentType, cancellationToken);

        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<FileStreamResult> DownloadFileAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await _fileService.DownloadAsync(id, cancellationToken);

        return File(response.Stream, response.ContentType);
    }

    [HttpPost("profile-pictures")]
    public async Task<ActionResult<Guid>> UploadProfilePictureAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var id = await _profilePictureService.UploadProfilePictureAsync(stream, file.ContentType, cancellationToken);

        return Ok(id);
    }

    [HttpGet("profile-pictures/{id}")]
    public async Task<FileStreamResult> DownloadProfilePictureAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await _profilePictureService.DownloadProfilePictureAsync(id, cancellationToken);

        return File(response.Stream, response.ContentType);
    }
}
