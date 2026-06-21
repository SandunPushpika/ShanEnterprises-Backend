namespace Core.DTOs.Request.Bookings;

public class BookingSearchRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
