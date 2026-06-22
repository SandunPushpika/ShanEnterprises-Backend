using Application.Interfaces.Services;
using Core.DTOs.Request;
using Core.DTOs.Response;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController:Controller
{
    private readonly IVehicleService _vehicleService;
    private readonly IBookingService _bookingService;

    public VehicleController(IVehicleService vehicleService, IBookingService bookingService)
    {
        _vehicleService = vehicleService;
        _bookingService = bookingService;
    }
    
    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> AddVehicle([FromBody] VehicleCreateRequest request)
    {
        await _vehicleService.AddVehicle(request);
        return new ApiResponse("Vehicle Added");
    }
    
    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>>UpdateVehicle(
        int id,
        [FromBody] VehicleUpdateRequest request)
    {
        await _vehicleService.UpdateVehicle(id, request);
        return new ApiResponse("Vehicle Updated");
    }
    
    [AllowAnonymous]
    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse>> SearchVehicles([FromBody] VehicleSearchRequest request)
    {
        var result = await _vehicleService.SearchVehicles(request);
        return new ApiResponse(data: result);
    }
    
    [AllowAnonymous]
    [HttpGet("brands")]
    public async Task<ActionResult<ApiResponse>> GetVehicleBrands()
    {
        var result = await _vehicleService.GetAllBrands();

        return new ApiResponse(data: result);
    }
    
    [AllowAnonymous]
    [HttpGet("types")]
    public async Task<ActionResult<ApiResponse>> GetVehicleTypes()
    {
        var result = await _vehicleService.GetAllVehicleTypes();
        return new ApiResponse(data: result);
    }
    
    [CustomAuthorize(UserRole.ADMIN)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteVehicle(int id)
    {
        await _vehicleService.DeleteVehicle(id);
        return new ApiResponse("Vehicle Deleted");
    }
    
    [AllowAnonymous]
    [HttpGet("{id}/images")]
    public async Task<ActionResult<ApiResponse>> GetVehicleImages(int id)
    {
        var result = await _vehicleService.GetVehicleImagesByVehicleIdAsync(id);
        return new ApiResponse(data: result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetVehicleById(int id)
    {
        var result = await _vehicleService.GetVehicleById(id);
        return new ApiResponse(data: result);
    }

    [AllowAnonymous]
    [HttpGet("{id}/booked-dates")]
    public async Task<ActionResult<ApiResponse>> GetBookedDates(int id)
    {
        var result = await _bookingService.GetBookedDatesByVehicleId(id);
        return new ApiResponse(data: result);
    }
    
}