namespace ChatApp.Services.Blobs;

public class FileRestrictionsManager
{
    public static readonly int MaxFileSize = 5 * 1024 * 1024; // 5 MB
    public static readonly string[] AllowedImageContentTypes = { "image/png", "image/jpeg" };
    public static readonly string[] AllowedFileContentTypes = { "video/mp4", "audio/mpeg", "image/png", "image/jpeg" };
    
    public bool IsContentTypeAllowed(string contentType, bool isImage)
    {
        return isImage ? AllowedImageContentTypes.Contains(contentType) : AllowedFileContentTypes.Contains(contentType);
    }
    
    public bool IsFileSizeAllowed(long fileSize)
    {
        return fileSize <= MaxFileSize;
    }
}