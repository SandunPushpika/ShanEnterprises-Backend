using Core.DTOs.Request.Common;
using Core.DTOs.Request.Review;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IReviewService
{
    Task AddReview(
        int vehicleId,
        AddReviewRequest request);

    Task<UserReviewResponse> GetUserReviews(int vehicleId);

    Task<SearchResponse<ReviewResponse>> GetReviews(int vehicleId, SearchRequest request);
}