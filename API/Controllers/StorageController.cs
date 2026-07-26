using Core.DTOs.Response;
using Core.Enums;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly AppSettings _appSettings;

    public StorageController(IStorageService storageService, IOptions<AppSettings> options)
    {
        _storageService = storageService;
        _appSettings = options.Value;
    }
    
    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> UploadBlob(IFormFile file, BlobType blobType, string fileName)
    {
        var res = await _storageService.UploadBlobAsync(file, blobType, connectionString: _appSettings.BlobConnectionString, fileName: fileName);
        return new ApiResponse(data:
        new {
            url = res,
        });
    }
    
    [Authorize]
    [HttpPost("multiple")]
    public async Task<ActionResult<ApiResponse>> UploadBlobs(List<IFormFile> files, BlobType blobType)
    {
        var res = await _storageService.UploadBlobsAsync(files, blobType, connectionString: _appSettings.BlobConnectionString);
        return new ApiResponse(data: res);
    }
}