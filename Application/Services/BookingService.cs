using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Entities;
using Core.Helpers;

namespace Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IMapper _mapper;

    public BookingService(
        IBookingRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task AddBooking(BookingCreateRequest request)
    {
        var isBooked = await _repository.IsBooked(request.VehicleId, request.PickupDateTime, request.ReturnDateTime);
        if (isBooked)
            throw new Exception("Vehicle is already booked on given date!");
        
        var booking = _mapper.Map<Booking>(request);

        booking.RentalDays =
            (booking.ReturnDatetime.Date - booking.PickupDatetime.Date).Days;

        booking.TotalAmount =
            booking.BaseRentalCost +
            booking.DriverFee +
            booking.TaxAmount -
            booking.DiscountAmount;

        booking.CreatedAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.BookingReference = RandomGenerator.GenerateBookingReference();

        await _repository.AddBooking(booking);
    }

    public Task UpdateBooking(int id, BookingUpdateRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<BookingReadResponse> GetBookingById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<SearchResponse<BookingReadResponse>> GetAllBookings(BookingSearchRequest request)
    {
        var (bookings, total) = await _repository.GetAllBookings(request);

        var bookingResponses = _mapper.Map<IReadOnlyCollection<BookingReadResponse>>(bookings);

        return new SearchResponse<BookingReadResponse>
        {
            Data       = bookingResponses,
            Total      = total,
            PageNumber = request.PageNumber,
            PageSize   = request.PageSize
        };
    }
    
    public Task UpdateBookingStatus(int id, BookingStatusUpdatRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBooking(int id)
    {
        throw new NotImplementedException();
    }
}