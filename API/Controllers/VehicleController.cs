using Application.Interfaces.Services;
using Core.DTOs.Request;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

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
}