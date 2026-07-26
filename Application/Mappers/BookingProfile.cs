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
            .ForMember(dest => dest.CustomerEmail,
                opt => opt.MapFrom(src => src.Customer.Email))
            .ForMember(dest => dest.CustomerPhone,
                opt => opt.MapFrom(src => src.Customer.PhoneNumber))
            .ForMember(dest => dest.VehicleModel,
                opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Model : string.Empty))
            .ForMember(dest => dest.VehicleRegistrationNumber,
                opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.RegistrationNumber : string.Empty))
            .ForMember(dest => dest.VehicleMainImageUrl,
                opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.MainImageUrl : string.Empty))
            .ForMember(dest => dest.BookingStatus,
                opt => opt.MapFrom(src => src.BookingStatus.ToString()));

        CreateMap<BookingCreateRequest, Booking>();
        CreateMap<BookingUpdateRequest, Booking>();
        CreateMap<BookingStatusUpdatRequest, Booking>();
    }
}