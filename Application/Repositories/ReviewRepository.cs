using Application.Interfaces.Repositories;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories;

public class ReviewRepository(AppDbContext context) : IReviewRepository
{
    public async Task<bool> HasUserReviewedVehicle(
        long customerId,
        int vehicleId)
    {
        return await context.Reviews.AnyAsync(r =>
            r.CustomerId == customerId &&
            r.VehicleId == vehicleId);
    }

    public async Task<Booking?> GetCustomerCompletedBooking(
        long customerId,
        int vehicleId)
    {
        return await context.Bookings
            .FirstOrDefaultAsync(b =>
                b.CustomerId == customerId &&
                b.VehicleId == vehicleId &&
                b.BookingStatus == BookingStatus.COMPLETED);
    }

    public async Task AddReview(Review review)
    {
        await context.Reviews.AddAsync(review);
        await context.SaveChangesAsync();
    }

    public async Task<decimal> GetVehicleAverageRating(
        int vehicleId)
    {
        var ratings = await context.Reviews
            .Where(r =>
                r.VehicleId == vehicleId &&
                r.VehicleRating.HasValue)
            .Select(r => r.VehicleRating!.Value)
            .ToListAsync();


        if (!ratings.Any())
            return 0;


        return (decimal)ratings.Average();
    }

    public async Task<Review> GetReviewByBookingId(int bookingId)
    {
        var review = await context.Reviews
            .Where(r => r.BookingId == bookingId)
            .FirstOrDefaultAsync();
        return review;
    }

    public async Task<SearchResponse<Review>> GetReviewsByVehicleId(
        int vehicleId,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = context.Reviews
            .Where(r => r.VehicleId == vehicleId);

        var totalCount = await query.CountAsync();

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new SearchResponse<Review>()
        {
            Data = reviews,
            Total = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}