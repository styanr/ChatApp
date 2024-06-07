using ImageMagick;

namespace ChatApp.Services.Blobs;

public class FileProcessor
{
    public Stream CompressImage(Stream imageStream, int quality)
    {
        using var image = CreateMagickImage(imageStream);
        SetImageFormatAndQuality(image, MagickFormat.Jpeg, quality);

        return SaveImageToStream(image);
    }
    
    public Stream ProcessProfilePicture(Stream imageStream)
    {
        using var image = CreateMagickImage(imageStream);
        CropToSquare(image);

        SetImageFormatAndQuality(image, MagickFormat.Jpeg, 80);

        return SaveImageToStream(image);
    }

    private MagickImage CreateMagickImage(Stream imageStream)
    {
        return new MagickImage(imageStream);
    }

    private void SetImageFormatAndQuality(MagickImage image, MagickFormat format, int quality)
    {
        image.Format = format;
        image.Quality = quality;
    }

    private void CropToSquare(MagickImage image)
    {
        var side = Math.Min(image.Width, image.Height);
        image.Crop(side, side, Gravity.Center);
    }

    private Stream SaveImageToStream(MagickImage image)
    {
        var memoryStream = new MemoryStream();
        image.Write(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }
}
