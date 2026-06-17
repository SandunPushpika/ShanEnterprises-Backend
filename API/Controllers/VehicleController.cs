using Application.Interfaces.Services;
using Core.DTOs.Request;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class VehicleController:Controller
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> AddVehicle([FromBody] VehicleCreateRequest request)
    {
        await _vehicleService.AddVehicle(request);
        return new ApiResponse("Vehicle Added");
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>>UpdateVehicle(
        int id,
        [FromBody] VehicleUpdateRequest request)
    {
        await _vehicleService.UpdateVehicle(id, request);
        return new ApiResponse("Vehicle Updated");
    }
    
    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse>> SearchVehicles([FromBody] VehicleSearchRequest request)
    {
        var result = await _vehicleService.SearchVehicles(request);
        return new ApiResponse(data: result);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<ApiResponse>> GetVehicleBrands()
    {
        var result = await _vehicleService.GetAllBrands();

        return new ApiResponse(data: result);
    }

    [HttpGet("types")]
    public async Task<ActionResult<ApiResponse>> GetVehicleTypes()
    {
        var result = await _vehicleService.GetAllVehicleTypes();
        return new ApiResponse(data: result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteVehicle(int id)
    {
        await _vehicleService.DeleteVehicle(id);
        return new ApiResponse("Vehicle Deleted");
    }
    
}