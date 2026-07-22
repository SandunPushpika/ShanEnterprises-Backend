namespace Core.DTOs.Request.Common;

public class SearchRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}