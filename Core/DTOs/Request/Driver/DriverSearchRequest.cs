using Core.Enums;

namespace Core.DTOs.Request.Driver;

public class DriverSearchRequest
{
    public DriverStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
