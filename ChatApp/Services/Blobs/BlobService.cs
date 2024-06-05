using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatApp.Models.Files;

namespace ChatApp.Services.Blobs;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    // TODO: store the container name in a configuration file
    private const string ContainerName = "files";

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var id = Guid.NewGuid();
        var blobClient = containerClient.GetBlobClient(id.ToString());

        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return id;
    }

    public async Task<FileResponse> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(id.ToString());

        var response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);

        return new FileResponse(response.Value.Content.ToStream(), response.Value.Details.ContentType);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(id.ToString());

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
