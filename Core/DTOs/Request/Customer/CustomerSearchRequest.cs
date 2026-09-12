using Core.Enums;

namespace Core.DTOs.Request.Customer;

public class CustomerSearchRequest
{
    public UserStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 8;
}