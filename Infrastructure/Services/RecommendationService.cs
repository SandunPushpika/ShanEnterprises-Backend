using System.Text;
using System.Text.Json;
using Core.DTOs.Response.Recommendation;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Exception = System.Exception;

namespace Infrastructure.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ILogger<RecommendationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    
    private readonly string _recommendationBaseUrl;
    private readonly string _recommendationApiKey;
    private readonly int _cacheDurationInMinutes = 30;
    
    public RecommendationService(
        ILogger<RecommendationService> logger,
        IOptions<AppSettings> options,
        HttpClient httpClient,
        IMemoryCache memoryCache
        )
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _recommendationBaseUrl = options.Value.RecommendationServiceUrl;
        _recommendationApiKey = options.Value.RecommendationServiceApiKey;
        _memoryCache = memoryCache;
        
        _httpClient.BaseAddress = new Uri(_recommendationBaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _recommendationApiKey);
        _httpClient.Timeout = TimeSpan.FromMinutes(1);
    }
    
    public async Task UpdateReview(int vehicleId)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(new {vehicle_id = vehicleId});
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("review", content);
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
        var cacheKey = $"recommendations_{query}_{limit}";
        var result = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheDurationInMinutes);
            
            return await GetVehicleRecommendationsFromApi(query, limit);
        });
        
        return result;
    }
    
    private async Task<RecommendationResponse?> GetVehicleRecommendationsFromApi(string query, int limit = 5)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(new {query = query, limit = limit});
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("vehicles/rank", content);
            var responseContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<RecommendationResponse>(responseContent);
            
            _logger.LogError($"review result status: {result.StatusCode}");
            _logger.LogError($"review result response: {responseContent}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
}