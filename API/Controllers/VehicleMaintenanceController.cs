using Application.Interfaces.Services;
using Core.DTOs.Request.Maintenance;
using Core.DTOs.Response;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
[CustomAuthorize(UserRole.ADMIN)]
public class VehicleMaintenanceController : ControllerBase
{
    private readonly IVehicleMaintenanceService _vehicleMaintenanceService;

    public VehicleMaintenanceController(IVehicleMaintenanceService vehicleMaintenanceService)
    {
        _vehicleMaintenanceService = vehicleMaintenanceService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse>> GetAllVehicleMaintenances()
    {
        var result = await _vehicleMaintenanceService.GetAllVehicleMaintenances();
        return new ApiResponse("Vehicle Maintenances Retrieved", data: result);
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> AddVehicleMaintenance([FromBody] MaintenanceCreateRequest request)
    {
        await _vehicleMaintenanceService.AddVehicleMaintenance(request);
        return new ApiResponse("Vehicle Maintenance Added");
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetVehicleMaintenanceById(int id)
    {
        var result = await _vehicleMaintenanceService.GetVehicleMaintenanceById(id);
        return new ApiResponse("Vehicle Maintenance Retrieved", data: result);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateVehicleMaintenance(int id, [FromBody] MaintenanceUpdateRequest request)
    {
        await _vehicleMaintenanceService.UpdateVehicleMaintenance(id, request);
        return new ApiResponse("Vehicle Maintenance Updated");
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteVehicleMaintenance(int id)
    {
        await _vehicleMaintenanceService.DeleteVehicleMaintenance(id);
        return new ApiResponse("Vehicle Maintenance Deleted");
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse>> SearchVehicleMaintenances([FromBody] MaintenanceSearchRequest request)
    {
        var result = await _vehicleMaintenanceService.SearchVehicleMaintenances(request);
        return new ApiResponse("Maintenance Records Retrieved", data: result);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse>> GetMaintenanceStats([FromQuery] string groupBy = "month", [FromQuery] int? year = null)
    {
        var result = await _vehicleMaintenanceService.GetMaintenanceStats(groupBy, year);
        return new ApiResponse("Maintenance Statistics Retrieved", data: result);
    }
}