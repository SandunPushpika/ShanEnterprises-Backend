using Core.DTOs.Response.Recommendation;

namespace Infrastructure.Interfaces;

public interface IRecommendationService
{
    Task UpdateReview(int vehicleId);
    Task<RecommendationResponse?> GetVehicleRecommendations(string query, int limit = 5);
}