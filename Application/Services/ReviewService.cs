using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Review;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;

namespace Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IContextService _contextService;
    private readonly IMapper _mapper;

    public ReviewService(
        IReviewRepository reviewRepository,
        IVehicleRepository vehicleRepository,
        IContextService contextService,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _vehicleRepository = vehicleRepository;
        _contextService = contextService;
        _mapper = mapper;
    }
    public async Task AddReview(int vehicleId, AddReviewRequest request)
    {
        var user = await _contextService.GetUser();
        
        var vehicle = await _vehicleRepository.GetVehicleById(vehicleId);

        if (vehicle == null || vehicle.Status == VehicleStatus.UNAVAILABLE)
        {
            throw new NotFoundException($"Vehicle with id {vehicleId} not found.");
        }
        
        var booking = await _reviewRepository.GetCustomerCompletedBooking(
            user.Id,
            vehicleId);

        if (booking == null)
        {
            throw new FailedOperationException(
                "You can only review a vehicle after completing a booking.");
        }
        
        var alreadyReviewed = await _reviewRepository.HasUserReviewedVehicle(
            user.Id,
            vehicleId);

        if (alreadyReviewed)
        {
            throw new FailedOperationException(
                "You have already reviewed this vehicle.");
        }


        var review = _mapper.Map<Review>(request);

        review.BookingId = booking.Id;
        review.CustomerId = user.Id;
        review.VehicleId = vehicleId;
        review.CreatedAt = DateTime.UtcNow;
        
        await _reviewRepository.AddReview(review);
        
        vehicle.AverageRating =
            await _reviewRepository.GetVehicleAverageRating(vehicleId);
        
    }
}