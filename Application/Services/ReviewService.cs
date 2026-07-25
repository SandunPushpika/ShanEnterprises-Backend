using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Common;
using Core.DTOs.Request.Review;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Infrastructure.Interfaces;

namespace Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IContextService _contextService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly IRecommendationService _recommendationService;

    public ReviewService(
        IReviewRepository reviewRepository,
        IVehicleRepository vehicleRepository,
        IContextService contextService,
        IBookingRepository bookingRepository,
        IMapper mapper,
        IRecommendationService recommendationService)
    {
        _reviewRepository = reviewRepository;
        _vehicleRepository = vehicleRepository;
        _contextService = contextService;
        _mapper = mapper;
        _bookingRepository = bookingRepository;
        _recommendationService = recommendationService;
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

        await _recommendationService.UpdateReview(vehicleId);
    }

    public async Task<UserReviewResponse> GetUserReviews(int vehicleId)
    {
        User user;
        try
        {
            user = await _contextService.GetUser();
            if (user == null)
                return new UserReviewResponse();
        }
        catch (Exception ex)
        {
            return new UserReviewResponse();
        }
        
        var booking = await _bookingRepository.GetBookingByVehicleIdAndUserId(vehicleId, (int)user.Id);
        if (booking == null)
            return new UserReviewResponse()
            {
                CanReview = false,
                HasReviewed = false,
                ReviewId = null
            };

        var review = await _reviewRepository.GetReviewByBookingId(booking.Id);
        if (review == null)
            return new UserReviewResponse()
            {
                CanReview = true,
                HasReviewed = false,
                ReviewId = null
            };
        
        return new UserReviewResponse()
        {
            CanReview = false,
            HasReviewed = true,
            ReviewId = review.Id,
        };
    }

    public async Task<SearchResponse<ReviewResponse>> GetReviews(int vehicleId, SearchRequest request)
    {
        var res = await _reviewRepository.GetReviewsByVehicleId(vehicleId, request.PageNumber, request.PageSize);

        var reviewResponses = res.Data.Select(r => new ReviewResponse()
        {
            Id = r.Id,
            Rating = r.VehicleRating,
            ReviewText = r.Comment,
            CreatedAt = r.CreatedAt,
        }).ToList();

        return new SearchResponse<ReviewResponse>()
        {
            Data = reviewResponses,
            Total = res.Total,
            PageNumber = res.PageNumber,
            PageSize = res.PageSize
        };
    }

    public async Task DeleteReview(int reviewId)
    {
        var review = await _reviewRepository.DeleteReview(reviewId);
        if(review?.VehicleId == null)
            return;
        
        await _recommendationService.UpdateReview((int) review.VehicleId);
    }
}