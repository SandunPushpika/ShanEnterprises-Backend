using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces;

public interface IImageCompressor
{
    Task<Stream> CompressAsync(IFormFile file);
}