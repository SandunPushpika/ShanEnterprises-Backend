using AutoMapper;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Entities;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingReadResponse>();
        CreateMap<BookingCreateRequest, Booking>();
        CreateMap<BookingUpdateRequest, Booking>();
        CreateMap<BookingStatusUpdatRequest, Booking>();
    }
}