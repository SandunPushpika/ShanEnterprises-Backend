using Core.DTOs.Response;
using Core.Entities;

namespace Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<bool> HasUserReviewedVehicle(
        long customerId,
        int vehicleId);

    Task<Booking?> GetCustomerCompletedBooking(
        long customerId,
        int vehicleId);

    Task AddReview(Review review);

    Task<decimal> GetVehicleAverageRating(
        int vehicleId);

    Task<Review> GetReviewByBookingId(int bookingId);

    Task<SearchResponse<Review>> GetReviewsByVehicleId(
        int vehicleId,
        int pageNumber = 1,
        int pageSize = 10);
}