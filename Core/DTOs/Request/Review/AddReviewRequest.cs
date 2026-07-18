namespace Core.DTOs.Request.Review;

public class AddReviewRequest
{
    public int VehicleRating { get; set; }

    public string? Comment { get; set; }
}