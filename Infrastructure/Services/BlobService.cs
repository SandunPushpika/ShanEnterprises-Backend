using Azure.Storage.Blobs;
using Core.Enums;
using Core.Exceptions;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class BlobService : IStorageService
{
    public async Task<string> UploadBlobAsync(IFormFile file, BlobType blobType, string connectionString, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(blobType.ToString().ToLower());
            await containerClient.CreateIfNotExistsAsync();
        
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(file.OpenReadStream());
        
            return blobClient.Uri.AbsoluteUri;
        }
        catch (Exception ex)
        {
            throw new FailedOperationException("Could not upload to Blob");
        }
    }
    
    public async Task<IEnumerable<string>> UploadBlobsAsync(
        List<IFormFile> files,
        BlobType blobType,
        string connectionString)
    {
        var list = new List<string>();
        foreach (var file in files)
        {
            var res = await UploadBlobAsync(
                file,
                blobType,
                connectionString,
                file.FileName);

            list.Add(res);
        }
        return list;
    }
}