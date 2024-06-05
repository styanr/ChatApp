using ChatApp.Services.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;
[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IBlobService _blobService;

    public FilesController(IBlobService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var id = await _blobService.UploadAsync(stream, file.ContentType, cancellationToken);

        return Ok(id);
    }

    [HttpGet("{id}")]
    public async Task<FileStreamResult> DownloadAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await _blobService.DownloadAsync(id, cancellationToken);

        return File(response.Stream, response.ContentType);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _blobService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
