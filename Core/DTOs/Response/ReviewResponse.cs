namespace Core.DTOs.Response;

public class ReviewResponse
{
    public int Id { get; set; }
    public string? ReviewText { get; set; }
    public int? Rating { get; set; }
}