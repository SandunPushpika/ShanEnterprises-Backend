using Azure.Storage.Blobs;
using Core.Enums;
using Core.Exceptions;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class BlobService : IStorageService
{
    private readonly IImageCompressor _imageCompressor;

    public BlobService(IImageCompressor imageCompressor)
    {
        _imageCompressor = imageCompressor;
    }
    
    public async Task<string> UploadBlobAsync(
        IFormFile file,
        BlobType blobType,
        string connectionString,
        string fileName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);

        try
        {
            var containerClient =
                blobServiceClient.GetBlobContainerClient(blobType.ToString().ToLower());

            await containerClient.CreateIfNotExistsAsync();

            Stream uploadStream;

            //Compress only images
            if (file.ContentType.StartsWith("image/"))
            {
                uploadStream = await _imageCompressor.CompressAsync(file);
            }
            else
            {
                uploadStream = file.OpenReadStream();
            }

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(uploadStream, overwrite: true);

            await uploadStream.DisposeAsync();

            return blobClient.Uri.AbsoluteUri;
        }
        catch
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
                GenerateFileName(file.FileName));

            list.Add(res);
        }
        return list;
    }

    private String GenerateFileName(string fileName)
    {
        return DateTime.Now.ToString("yyyyMMddHHmmssfff") + fileName;
    }
}