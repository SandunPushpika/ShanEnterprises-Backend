using AutoMapper;
using Core.DTOs.Request.Driver;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Mappers;

public class DriverProfile : Profile
{
    public DriverProfile()
    {
        CreateMap<DriverCreateRequest, Driver>();

        CreateMap<Driver, DriverResponse>()
            .ForMember(dest => dest.DriverName,
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.UserEmail,
                opt => opt.MapFrom(src => src.User.Email));
    }
}
