using Core.DTOs.Request.Review;

namespace Application.Interfaces.Services;

public interface IReviewService
{
    Task AddReview(
        int vehicleId,
        AddReviewRequest request);
}