using Application.Interfaces.Services;
using Core.DTOs.Request.Driver;
using Core.DTOs.Response;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpPost("request")]
    public async Task<ActionResult<ApiResponse>> SubmitDriverRequest(
        [FromBody] DriverCreateRequest createRequest)
    {
        await _driverService.SubmitDriverRequestAsync(createRequest);
        return new ApiResponse("Driver request submitted successfully.");
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPut("{driverId}/approve")]
    public async Task<ActionResult<ApiResponse>> ApproveDriver(int driverId)
    {
        await _driverService.ApproveDriverAsync(driverId);
        return new ApiResponse("Driver approved successfully.");
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPut("{driverId}/reject")]
    public async Task<ActionResult<ApiResponse>> RejectDriver(int driverId)
    {
        await _driverService.RejectDriverAsync(driverId);
        return new ApiResponse("Driver request rejected / driver deactivated successfully.");
    }

    [HttpPost("search")]
    public async Task<ActionResult<ApiResponse>> GetDriversByStatus(
        [FromBody] DriverSearchRequest request)
    {
        var result = await _driverService.GetDriversByStatusAsync(request);
        return new ApiResponse(data: result);
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse>> GetDriverStats()
    {
        var result = await _driverService.GetDriverStatsAsync();
        return new ApiResponse(data: result);
    }
    
    [HttpGet("my-status")]
    public async Task<ActionResult<ApiResponse>> GetMyDriverStatus()
    {
        var result = await _driverService.GetMyDriverStatusAsync();
        return new ApiResponse(data: result);
    }

    [HttpPost("available")]
    public async Task<ActionResult<ApiResponse>> GetAvailableDrivers(
        [FromBody] AvailableDriverRequest request)
    {
        var result = await _driverService.GetAvailableDriversAsync(request);
        return new ApiResponse(data: result);
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpGet("{driverId}")]
    public async Task<ActionResult<ApiResponse>> GetDriverById(int driverId)
    {
        var result = await _driverService.GetDriverByIdAsync(driverId);
        return new ApiResponse(data: result);
    }

    [HttpGet("my-trips")]
    public async Task<ActionResult<ApiResponse>> GetMyTrips([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _driverService.GetMyTripsAsync(pageNumber, pageSize);
        return new ApiResponse(data: result);
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpGet("{driverId}/trips")]
    public async Task<ActionResult<ApiResponse>> GetDriverTrips(int driverId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _driverService.GetDriverTripsAsync(driverId, pageNumber, pageSize);
        return new ApiResponse(data: result);
    }

    [HttpPut("trips/{bookingId}/cancel")]
    public async Task<ActionResult<ApiResponse>> CancelTripAssignment(int bookingId)
    {
        var result = await _driverService.CancelTripAssignmentAsync(bookingId);
        return new ApiResponse(message: result.Message, data: result);
    }
}
