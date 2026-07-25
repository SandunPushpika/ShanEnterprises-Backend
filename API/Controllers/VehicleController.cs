using Application.Interfaces.Services;
using Core.DTOs.Request;
using Core.DTOs.Request.Common;
using Core.DTOs.Request.Review;
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
    private readonly IReviewService _reviewService;

    public VehicleController(IVehicleService vehicleService, IBookingService bookingService,IReviewService reviewService)
    {
        _vehicleService = vehicleService;
        _bookingService = bookingService;
        _reviewService = reviewService;
        
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
    
    [CustomAuthorize(UserRole.CUSTOMER)]
    [HttpPost("{vehicleId}/add-review")]
    public async Task<ActionResult<ApiResponse>> AddReview(
        int vehicleId,
        [FromBody] AddReviewRequest request)
    {
        await _reviewService.AddReview(
            vehicleId,
            request);

        return new ApiResponse(
            "Review added successfully");
    }

    [AllowAnonymous]
    [HttpPost("{vehicleId}/reviews")]
    public async Task<ActionResult<ApiResponse>> GetVehicleReviews(int vehicleId, SearchRequest request)
    {
        var res = await _reviewService.GetReviews(vehicleId, request);
        return new ApiResponse(data: res);
    }

    [AllowAnonymous]
    [HttpGet("{id}/review-user")]
    public async Task<ActionResult<ApiResponse>> GetReview(int id)
    {
        var res = await _reviewService.GetUserReviews(id);
        return new ApiResponse(data: res);
    }
    
    [AllowAnonymous]
    [HttpDelete("{vehicleId}/reviews/{reviewId}")]
    public async Task<ActionResult<ApiResponse>> DeleteReview(int vehicleId, int reviewId)
    {
        await _reviewService.DeleteReview(reviewId);
        return new ApiResponse(data: "Review deleted");
    }
}