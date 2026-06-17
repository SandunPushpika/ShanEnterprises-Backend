using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services;

public class ImageCompressor : IImageCompressor
{
    public async Task<Stream> CompressAsync(IFormFile file)
    {
        var outputStream = new MemoryStream();

        await using (var inputStream = file.OpenReadStream())
        using (var image = await Image.LoadAsync(inputStream))
        {
            const int maxWidth = 1280;

            if (image.Width > maxWidth)
            {
                var ratio = (float)maxWidth / image.Width;
                var newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(maxWidth, newHeight));
            }
            
            var encoder = new JpegEncoder
            {
                Quality = 75
            };

            await image.SaveAsJpegAsync(outputStream, encoder);
        }

        outputStream.Position = 0;
        return outputStream;
    }
    
}