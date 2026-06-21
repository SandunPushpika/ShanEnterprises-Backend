using AutoMapper;
using Core.DTOs.Request.Bookings;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Mappers;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingReadResponse>()
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"))
            .ForMember(dest => dest.VehicleModel,
                opt => opt.MapFrom(src => src.Vehicle.Model))
            .ForMember(dest => dest.PickupDateTime,
                opt => opt.MapFrom(src => src.PickupDatetime))
            .ForMember(dest => dest.ReturnDateTime,
                opt => opt.MapFrom(src => src.ReturnDatetime))
            .ForMember(dest => dest.BookingStatus,
                opt => opt.MapFrom(src => src.BookingStatus.ToString()));

        CreateMap<BookingCreateRequest, Booking>();
        CreateMap<BookingUpdateRequest, Booking>();
        CreateMap<BookingStatusUpdatRequest, Booking>();
    }
}