using Application.Interfaces.Services;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> AddBooking(
        [FromBody] BookingCreateRequest request)
    {
        await _bookingService.AddBooking(request);

        return new ApiResponse("Booking Added");
    }

    [CustomAuthorize(UserRole.ADMIN)]
    [HttpPost("all")]
    public async Task<ActionResult<ApiResponse>> GetAllBookings([FromBody] BookingSearchRequest request)
    {
        var result = await _bookingService.GetAllBookings(request);
        return new ApiResponse(data: result);
    }

    [HttpPost("user-booking")]
    public async Task<ActionResult<ApiResponse>> GetBookingsForUser([FromBody] BookingSearchRequest request)
    {
        var result = await _bookingService.GetAllBookingsForUser(request);
        return new ApiResponse(data: result);
    }
}