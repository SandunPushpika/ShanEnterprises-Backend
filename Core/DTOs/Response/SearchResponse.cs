using System.Collections.Generic;

namespace Core.DTOs.Response;

public class SearchResponse<T>
{
    public IReadOnlyCollection<T> Data { get; set; } = new List<T>();
    public int Total { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}