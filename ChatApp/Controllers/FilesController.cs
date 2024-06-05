using ChatApp.Services.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;
[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IProfilePictureService _profilePictureService;

    public FilesController(IProfilePictureService profilePictureService)
    {
        _profilePictureService = profilePictureService;
    }

    [HttpPost("profile-pictures")]
    public async Task<ActionResult<Guid>> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var id = await _profilePictureService.UploadProfilePictureAsync(stream, file.ContentType, cancellationToken);

        return Ok(id);
    }

    [HttpGet("profile-pictures/{id}")]
    public async Task<FileStreamResult> DownloadAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await _profilePictureService.DownloadProfilePictureAsync(id, cancellationToken);

        return File(response.Stream, response.ContentType);
    }
}
