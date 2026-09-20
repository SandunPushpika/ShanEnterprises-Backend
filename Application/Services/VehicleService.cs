using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request;
using Core.Entities;
using Core.Exceptions;
using Core.DTOs.Response;
using Core.DTOs.Response.Recommendation;
using Core.Enums;
using Infrastructure.Interfaces;

namespace Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repository;
    private readonly IRecommendationService _recommendationService;
    private readonly IMapper _mapper;
    
    public VehicleService(IVehicleRepository repository, IRecommendationService recommendationService,IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _recommendationService = recommendationService;
    }

    public async Task AddVehicle(VehicleCreateRequest request)
    {
        var vehicle = _mapper.Map<Vehicle>(request);
        vehicle.CreatedAt = DateTime.UtcNow;
        vehicle.UpdatedAt = DateTime.UtcNow;
        
        var vehicleId = await _repository.AddVehicle(vehicle);
        
        if(request.ImageUrls == null)
            return;

        await AddVehicleImages(request.ImageUrls, vehicleId);
    }

    public async Task UpdateVehicle(int id, VehicleUpdateRequest request)
    {
        var vehicle = await _repository.GetVehicleById(id);

        if (vehicle == null)
            throw new NotFoundException($"Vehicle with id {id} not found");

        _mapper.Map(request, vehicle);

        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);

        await _repository.UpdateVehicle(vehicle);
        
        if(request.ImageUrls == null)
            return;
        
        await AddVehicleImages(request.ImageUrls, vehicle.Id);
    }
    
    public async Task<SearchResponse<VehicleResponse>> SearchVehicles(VehicleSearchRequest request)
    {
        var (vehicles, total) = await _repository.SearchVehicles(request);

        RecommendationResponse? recommendedVehicles = null;
        if(!string.IsNullOrWhiteSpace(request.Search))
            recommendedVehicles = await _recommendationService.GetVehicleRecommendations(request.Search);

        if (recommendedVehicles == null || recommendedVehicles.Recommendations.Count == 0 || recommendedVehicles.Recommendations[0].Similarity < 0.5)
        {
            var vehicleResponses = _mapper.Map<IReadOnlyCollection<VehicleResponse>>(vehicles);
            return new SearchResponse<VehicleResponse>
            {
                Data = vehicleResponses,
                Total = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        request.Search = null;
        request.VehicleIds = recommendedVehicles.Recommendations.Select(v => v.VehicleId).ToList();
        var (recommended, _) = await _repository.SearchVehicles(request);
        
        return MapVehicleArrays(
            vehicles,
            recommended,
            request,
            total
        );
    }

    public async Task<IReadOnlyCollection<VehicleBrand>> GetAllBrands()
    {
        return await _repository.GetAllVehicleBrands();
    }

    public async Task<IReadOnlyCollection<VehicleType>> GetAllVehicleTypes()
    {
        return await _repository.GetAllVehicleTypes();
    }

    public async Task DeleteVehicle(int id)
    {
        var vehicle = await _repository.GetVehicleById(id);
        if(vehicle == null || vehicle.IsDeleted)
            throw new NotFoundException($"Vehicle with id {id} not found");

        vehicle.IsDeleted = true;
        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);
        
        await _repository.UpdateVehicle(vehicle);
    }

    public async Task<List<VehicleImages>> GetVehicleImagesByVehicleIdAsync(int vehicleId)
    {
        return await _repository.GetVehicleImagesByVehicleIdAsync(vehicleId);
    }

    public async Task<VehicleResponse> GetVehicleById(int vehicleId)
    {
        var vehicle = await _repository.GetVehicleById(vehicleId, true, true, true);
        return _mapper.Map<VehicleResponse>(vehicle);
    }

    public async Task SetVehicleAvailability(int id, bool makeUnavailable)
    {
        var vehicle = await _repository.GetVehicleById(id);
        if (vehicle == null)
            throw new NotFoundException($"Vehicle with id {id} not found");
        
        if (makeUnavailable)
        {
            if (vehicle.Status == VehicleStatus.UNAVAILABLE)
                throw new FailedOperationException("Vehicle is already unavailable.");
            vehicle.Status = VehicleStatus.UNAVAILABLE;
        }
        else
        {
            if (vehicle.Status != VehicleStatus.UNAVAILABLE)
                throw new FailedOperationException("Vehicle is already available.");
            vehicle.Status = VehicleStatus.AVAILABLE;
        }
        
        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.CreatedAt = DateTime.SpecifyKind(vehicle.CreatedAt, DateTimeKind.Utc);
        await _repository.UpdateVehicle(vehicle);
    }
    
    #region private methods

    private async Task AddVehicleImages(IReadOnlyList<string> imageUrls, int vehicleId)
    {
        await _repository.DeleteVehicleImagesByVehicleAsync(vehicleId);
        
        var images = imageUrls.Distinct().ToList()
            .Select<string, VehicleImages>(img => new VehicleImages()
            {
                VehicleId = vehicleId,
                ImageUrl = img
            });
        await _repository.AddVehicleImagesAsync(images);
    }

    private SearchResponse<VehicleResponse> MapVehicleArrays(
        IReadOnlyCollection<Vehicle> dbVehicles,
        IReadOnlyCollection<Vehicle> recommendedVehicles,
        VehicleSearchRequest request,
        int total)
    {
        var topRecommendations = recommendedVehicles
            .Take(3)
            .ToList();

        var remainingVehicles = dbVehicles
            .Where(v => topRecommendations.All(r => r.Id != v.Id))
            .ToList();

        var mergedVehicles = topRecommendations
            .Concat(remainingVehicles)
            .Take(request.PageSize)
            .ToList();

        var response = _mapper.Map<List<VehicleResponse>>(mergedVehicles);

        return new SearchResponse<VehicleResponse>
        {
            Data = response,
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<VehicleStatsResponse> GetVehicleStatsAsync()
    {
        return await _repository.GetVehicleStatsAsync();
    }
    
    #endregion
}