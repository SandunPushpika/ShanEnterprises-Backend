using Application.Interfaces.Services;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]

public class BookingController: ControllerBase
{
    private readonly IBookingService _bookingService;
    
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;   
    }


    [HttpPost("add-booking")]
    public async Task<ActionResult<ApiResponse>> AddBooking(
        [FromBody] BookingCreateRequest request)
    {
        await _bookingService.AddBooking(request);

        return new ApiResponse("Booking Added");
    }
}