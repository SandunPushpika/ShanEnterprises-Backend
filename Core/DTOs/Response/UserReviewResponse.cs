namespace Core.DTOs.Response;

public class UserReviewResponse
{
    public bool HasReviewed { get; set; }
    public bool CanReview { get; set; }
    public int? ReviewId { get; set; }
}