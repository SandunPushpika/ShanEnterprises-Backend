using System.Text.Json.Serialization;

namespace Core.DTOs.Response.Recommendation;

public class RecommendationResponse
{
    [JsonPropertyName("results")]
    public List<RecommendationResponseItem> Recommendations { get; set; }
}