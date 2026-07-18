using Application.Interfaces.Services;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShanEnterprises.Attributes;

namespace ShanEnterprises.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> AddBooking(
        [FromBody] BookingCreateRequest request)
    {
        var paymentUrl = await _bookingService.AddBooking(request);

        return new ApiResponse(data: new {Url = paymentUrl});
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

    [HttpGet("verify-booking")]
    public async Task<ActionResult<ApiResponse>> VerifyBooking(string sessionId)
    {
        var result = await _bookingService.VerifyBooking(sessionId);
        return new ApiResponse(data: new { IsPaid = result });
    }

    [HttpDelete("{bookingId}")]
    public async Task<ActionResult<ApiResponse>> DeleteBooking(int bookingId)
    {
        await _bookingService.DeleteBooking(bookingId);
        return new ApiResponse("Booking cancelled successfully!");
    }
}