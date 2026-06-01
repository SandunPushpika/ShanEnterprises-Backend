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
    
    [HttpPost("add-vehicle")]
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
}