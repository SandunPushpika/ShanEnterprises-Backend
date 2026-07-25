using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Request.Payment;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Helpers;
using Infrastructure.Interfaces;

namespace Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;
    private readonly IContextService _context;
    private readonly IPaymentService _paymentService;

    private const int _advancedPaymentPercentage = 40;

    public BookingService(
        IBookingRepository repository,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        IContextService context,
        IPaymentService paymentService,
        IPaymentRepository paymentRepository
        )
    {
        _repository = repository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _context = context;
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
    }

    public async Task<string> AddBooking(BookingCreateRequest request)
    {
        var user = await _context.GetUser();
        if (user == null)
            throw new UnauthorizedAccessException("User is not logged in to place the booking");
        
        var isBooked = await _repository.IsBooked(request.VehicleId, request.PickupDateTime, request.ReturnDateTime);
        if (isBooked)
            throw new Exception("Vehicle is already booked on given date!");
        
        var booking = _mapper.Map<Booking>(request);

        booking.CustomerId = user.Id;
        booking.RentalDays =
            (booking.ReturnDatetime.Date - booking.PickupDatetime.Date).Days;

        booking.TotalAmount =
            booking.BaseRentalCost +
            booking.DriverFee +
            booking.TaxAmount -
            booking.DiscountAmount;

        booking.CreatedAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        booking.BookingStatus = BookingStatus.PENDING;
        booking.BookingReference = RandomGenerator.GenerateBookingReference();
        
        var bookRes = await _repository.AddBooking(booking);
        
        await _repository.SaveAsync();

        try
        {
            var externalPaymentResponse = await GenerateCheckoutSession(bookRes);
            await _repository.SaveAsync();
            return externalPaymentResponse.PaymentUrl;
        }
        catch (Exception)
        {
            await _repository.DeleteBooking(bookRes);
            throw;
        }
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

    public async Task<IReadOnlyCollection<BookedDateRangeResponse>> GetBookedDatesByVehicleId(int vehicleId)
    {
        var vehicle = await _vehicleRepository.GetVehicleById(vehicleId);
        if (vehicle == null)
            throw new NotFoundException($"Vehicle with id {vehicleId} not found");

        return await _repository.GetBookedDatesByVehicleId(vehicleId);
    }

    public async Task<SearchResponse<BookingReadResponse>> GetAllBookingsForUser(BookingSearchRequest request)
    {
        var user = await _context.GetUser();
        if (user == null)
            throw new UnauthorizedAccessException();
        
        request.CustomerId = user.Id;
        return await GetAllBookings(request);
    }

    public async Task<bool> VerifyBooking(string sessionId)
    {
        var payment = await _paymentRepository.GetPaymentByReference(sessionId);
        if(payment == null)
            throw new NotFoundException("Payment not found");
        
        if(payment.PaymentStatus == PaymentStatus.COMPLETED)
            return true;

        var (paymentIntent, status) = await _paymentService.IsPaid(sessionId);
        
        payment.TransactionReference = paymentIntent;
        payment.PaymentStatus = status;
        payment.PaidAt = DateTime.UtcNow;
        payment.CreatedAt = DateTime.SpecifyKind(payment.CreatedAt, DateTimeKind.Utc);;

        var booking = await _repository.GetBookingById((int)payment.BookingId);
        if (booking == null)
            throw new NotFoundException($"Booking with id {payment.BookingId} not found");

        if (status == PaymentStatus.COMPLETED)
            booking.BookingStatus = BookingStatus.CONFIRMED;
        
        booking.UpdatedAt = DateTime.UtcNow;
        booking.CreatedAt = DateTime.SpecifyKind(booking.CreatedAt, DateTimeKind.Utc);
        booking.PickupDatetime = DateTime.SpecifyKind(booking.PickupDatetime, DateTimeKind.Utc);
        booking.ReturnDatetime = DateTime.SpecifyKind(booking.ReturnDatetime, DateTimeKind.Utc);
        await _repository.UpdateBooking(booking);
        await _paymentRepository.UpdatePayment(payment);
        
        return status == PaymentStatus.COMPLETED;
    }

    private async Task<ExternalPaymentResponse> GenerateCheckoutSession(Booking booking)
    {
        var vehicle = await _vehicleRepository.GetVehicleById(booking.VehicleId);
        if(vehicle == null)
            throw new NotFoundException($"Vehicle with id {booking.VehicleId} not found");

        var amountToPay = (long)(booking.TotalAmount * _advancedPaymentPercentage / (decimal)100.0);
        var externalPaymentRequest = new ExternalPaymentRequest()
        {
            BookingReference = booking.BookingReference,
            Amount = amountToPay,
            To = booking.ReturnDatetime,
            From = booking.PickupDatetime,
            Vehicle = vehicle.Model
        };
        var checkoutSession = await _paymentService.CreateCheckoutSession(externalPaymentRequest);

        var payment = new Payments()
        {
            Amount = (decimal)amountToPay,
            CustomerId = booking.CustomerId,
            BookingId = booking.Id,
            CreatedAt = DateTime.UtcNow,
            TransactionReference = checkoutSession.ExternalPaymentId,
            PaymentMethod = PaymentMethod.ONLINE
        };
        
        await _paymentRepository.AddPayment(payment);
        
        return checkoutSession;
    }
}