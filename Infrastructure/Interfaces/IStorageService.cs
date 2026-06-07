using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces;

public interface IStorageService
{
    Task<string> UploadBlobAsync(IFormFile file, BlobType blobType, string connectionString, string fileName);

    Task<IEnumerable<string>> UploadBlobsAsync(
        List<IFormFile> files,
        BlobType blobType,
        string connectionString);
}