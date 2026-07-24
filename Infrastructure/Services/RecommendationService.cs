using System.Text.Json;
using Core.DTOs.Response.Recommendation;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Exception = System.Exception;

namespace Infrastructure.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ILogger<RecommendationService> _logger;
    private readonly string _recommendationBaseUrl;
    private readonly string _recommendationApiKey;
    private readonly HttpClient _httpClient;
    
    public RecommendationService(
        ILogger<RecommendationService> logger,
        IOptions<AppSettings> options,
        HttpClient httpClient
        )
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _recommendationBaseUrl = options.Value.RecommendationServiceUrl;
        _recommendationApiKey = options.Value.RecommendationServiceApiKey;
        
        _httpClient.BaseAddress = new Uri(_recommendationBaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _recommendationApiKey);
    }
    
    public async Task UpdateReview(int vehicleId)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(new {vehicle_id = vehicleId});
            var result = await _httpClient.PostAsync("/review", new StringContent(jsonString));
            var responseContent = await result.Content.ReadAsStringAsync();
            
            _logger.LogInformation($"review result status: {result.StatusCode}");
            _logger.LogInformation($"review result response: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task<RecommendationResponse?> GetVehicleRecommendations(string query, int limit = 5)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(new {query = query, limit = limit});
            var result = await _httpClient.PostAsync("/vehicles/rank", new StringContent(jsonString));
            var responseContent = await result.Content.ReadAsStringAsync();
            
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError($"review result status: {result.StatusCode}");
                _logger.LogError($"review result response: {responseContent}");
            }
            
            return JsonSerializer.Deserialize<RecommendationResponse>(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
}