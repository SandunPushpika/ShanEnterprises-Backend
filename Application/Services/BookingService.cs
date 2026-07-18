using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Request.Other;
using Core.DTOs.Request.Payment;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Helpers;
using Core.Helpers.Templates;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;
    private readonly IContextService _context;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly AppSettings _appSettings;
    private readonly IUserRepository _userRepository;

    private const int _advancedPaymentPercentage = 40;
    private const int _refundForLessThan7 = 0;
    private const int _refundForMoreThan7LessThan14 = 50;
    private const int _refundForMoreThan14 = 100;

    public BookingService(
        IBookingRepository repository,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        IContextService context,
        IPaymentService paymentService,
        IPaymentRepository paymentRepository,
        IEmailService emailService,
        IOptions<AppSettings> options,
        IUserRepository userRepository
        )
    {
        _repository = repository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _context = context;
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
        _emailService = emailService;
        _appSettings = options.Value;
        _userRepository = userRepository;
    }

    public async Task<string> AddBooking(BookingCreateRequest request)
    {
        var user = await _context.GetUser();
        if (user == null)
            throw new UnauthorizedAccessException("User is not logged in to place the booking");
        
        var vehicle = await _vehicleRepository.GetVehicleById(request.VehicleId);
        if (vehicle == null || vehicle.Status == VehicleStatus.MAINTENANCE)
            throw new Exception("Unable to book this vehicle, Please select another vehicle");
        
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
            var url =  externalPaymentResponse.PaymentUrl;

            await SendBookingPaymentRequestEmail(user.FirstName, booking.BookingReference, vehicle.Model,
                GeneratePaymentAmount(booking).ToString(), url, user.Email);
            
            return url;
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

    public async Task DeleteBooking(int id)
    {
        var booking = await _repository.GetBookingById(id);
        if (booking == null)
            throw new NotFoundException("Booking not found");
        
        booking.CreatedAt = DateTime.SpecifyKind(booking.CreatedAt, DateTimeKind.Utc);
        booking.UpdatedAt = DateTime.UtcNow;
        booking.PickupDatetime = DateTime.SpecifyKind(booking.PickupDatetime, DateTimeKind.Utc);
        booking.ReturnDatetime = DateTime.SpecifyKind(booking.ReturnDatetime, DateTimeKind.Utc);
        
        
        if(booking.BookingStatus == BookingStatus.COMPLETED)
            throw new Exception("You cannot cancel this booking!");
        var payment = await _paymentRepository.GetPaymentByBookingId(booking.Id);
        
        if (booking.BookingStatus != BookingStatus.CONFIRMED || payment == null || payment.PaymentStatus != PaymentStatus.COMPLETED)
        {
            booking.BookingStatus = BookingStatus.CANCELLED;
            await _repository.UpdateBooking(booking);
            return;
        }

        var refundPercentage = GetRefundPercentage(booking.PickupDatetime);
        var refundAmount = (payment.Amount * refundPercentage) / (decimal)100.0;
        if (refundAmount > 0)
            await _paymentService.MakeRefund(new RefundRequest()
            {
                Amount = (long)refundAmount,
                Currency = "lkr",
                PaymentId = payment.TransactionReference
            });
        
        booking.BookingStatus = BookingStatus.CANCELLED;
        await _repository.UpdateBooking(booking);
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
        var user = await _context.GetUser();
        var payment = await _paymentRepository.GetPaymentByReference(sessionId);
        if(payment == null)
            throw new NotFoundException("Payment not found");
        
        var booking = await _repository.GetBookingById((int)payment.BookingId);
        if (booking == null)
            throw new NotFoundException($"Booking with id {payment.BookingId} not found");
        
        if(payment.PaymentStatus == PaymentStatus.COMPLETED)
            return true;

        var (paymentIntent, status) = await _paymentService.IsPaid(sessionId);
        
        payment.TransactionReference = paymentIntent;
        payment.PaymentStatus = status;
        payment.PaidAt = DateTime.UtcNow;
        payment.CreatedAt = DateTime.SpecifyKind(payment.CreatedAt, DateTimeKind.Utc);;

        if (status == PaymentStatus.COMPLETED)
            booking.BookingStatus = BookingStatus.CONFIRMED;
        
        booking.UpdatedAt = DateTime.UtcNow;
        booking.CreatedAt = DateTime.SpecifyKind(booking.CreatedAt, DateTimeKind.Utc);
        booking.PickupDatetime = DateTime.SpecifyKind(booking.PickupDatetime, DateTimeKind.Utc);
        booking.ReturnDatetime = DateTime.SpecifyKind(booking.ReturnDatetime, DateTimeKind.Utc);
        await _repository.UpdateBooking(booking);
        await _paymentRepository.UpdatePayment(payment);
        
        if(status != PaymentStatus.COMPLETED)
            return false;
        
        var emailSendRequest = new EmailSendRequest()
        {
            To = user.Email,
            Body = BookingEmailTemplates.GenerateBookingConfirmed(
                user.FirstName,
                booking.BookingReference,
                booking.Vehicle.Model,
                booking.PickupDatetime.ToShortDateString(),
                booking.ReturnDatetime.ToShortDateString()
                ),
            Subject = "Booking Confirmation - " + booking.BookingReference,
            IsBodyHtml = true
        };
        await _emailService.SendEmailAsync(_appSettings.MailSettings, emailSendRequest);
        
        return true;
    }

    public async Task CompleteBooking(int bookingId)
    {
        var booking = await _repository.GetBookingById(bookingId);
        if(booking == null)
            throw new NotFoundException($"Booking with id {bookingId} not found");
        
        var user = await _userRepository.GetUserByIdAsync((int)booking.CustomerId);
        if(user == null)
            throw new NotFoundException($"User with id {bookingId} not found");
        
        booking.CreatedAt = DateTime.SpecifyKind(booking.CreatedAt, DateTimeKind.Utc);
        booking.UpdatedAt = DateTime.UtcNow;
        booking.PickupDatetime = DateTime.SpecifyKind(booking.PickupDatetime, DateTimeKind.Utc);
        booking.ReturnDatetime = DateTime.SpecifyKind(booking.ReturnDatetime, DateTimeKind.Utc);
        booking.BookingStatus = BookingStatus.COMPLETED;
        
        await _repository.UpdateBooking(booking);

        var emailRequest = new EmailSendRequest()
        {
            To = user.Email,
            Body = BookingEmailTemplates.GenerateTripCompleted(user.FirstName, booking.BookingReference,
                booking.Vehicle.Model, _appSettings.RatingUrl+$"{booking.VehicleId}"),
            Subject = "DriveLux: Thank you riding with us!",
            IsBodyHtml = true,
            ReciepientName = user.FirstName,
        };
        
        await _emailService.SendEmailAsync(_appSettings.MailSettings, emailRequest);
    }

    #region Private methods
    
    private async Task<ExternalPaymentResponse> GenerateCheckoutSession(Booking booking)
    {
        var vehicle = await _vehicleRepository.GetVehicleById(booking.VehicleId);
        if(vehicle == null)
            throw new NotFoundException($"Vehicle with id {booking.VehicleId} not found");

        var amountToPay = GeneratePaymentAmount(booking);
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

    private long GeneratePaymentAmount(Booking booking)
    {
        return (long)(booking.TotalAmount * _advancedPaymentPercentage / (decimal)100.0);
    }

    private Task SendBookingPaymentRequestEmail(string customerName, string bookingReference, string vehicleModel, string paymentAmount, string paymentLink, string email)
    {
        var emailSendRequest = new EmailSendRequest()
        {
            Body = BookingEmailTemplates.GeneratePaymentRequest(customerName, bookingReference, vehicleModel,
                paymentAmount, paymentLink),
            Subject = "Payment Request For Booking Confirmation",
            IsBodyHtml = true,
            To = email,
            ReciepientName = customerName,
        };
        return _emailService.SendEmailAsync(_appSettings.MailSettings, emailSendRequest);
    }

    private decimal GetRefundPercentage(DateTime pickupDateTime)
    {
        var today = DateTime.UtcNow.Date;
        var pickupDate = pickupDateTime.Date;

        var daysBeforePickup = (pickupDate - today).Days;

        decimal refundPercentage;

        if (daysBeforePickup > 14)
        {
            refundPercentage = _refundForMoreThan14;
        }
        else if (daysBeforePickup >= 7 && daysBeforePickup <= 14)
        {
            refundPercentage = _refundForMoreThan7LessThan14;
        }
        else
        {
            refundPercentage = _refundForLessThan7;
        }
        
        return refundPercentage;
    }
    
    #endregion
}