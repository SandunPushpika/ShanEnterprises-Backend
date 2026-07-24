using System.Text.Json.Serialization;

namespace Core.DTOs.Response.Recommendation;

public class RecommendationResponseItem
{
    [JsonPropertyName("vehicle_id")]
    public int VehicleId { get; set; }
    
    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }
}