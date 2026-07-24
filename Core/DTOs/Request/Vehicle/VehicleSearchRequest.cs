using Core.DTOs.Response.Recommendation;
using Core.Enums;

namespace Core.DTOs.Request;

public class VehicleSearchRequest
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? TypeId { get; set; }
    public VehicleStatus? Status { get; set; }
    public int? MinPassengers { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Search { get; set; }
    public List<int>? VehicleIds { get; set; }
}

