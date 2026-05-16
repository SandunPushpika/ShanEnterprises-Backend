namespace Core.DTOs.Response;

public class ApiResponse(string? message = null, bool success = true, object? data = null)
{
    public string? Message { get; set; } = message;
    public bool Success { get; set; } = success;
    public object? Data { get; set; } = data;
}