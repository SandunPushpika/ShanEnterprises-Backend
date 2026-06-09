using AutoMapper;
using Core.DTOs.Request;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Mappers;

public class VehicleProfile: Profile
{
    public VehicleProfile()
    {
        CreateMap<VehicleCreateRequest, Vehicle>();
        CreateMap<VehicleUpdateRequest, Vehicle>();
        CreateMap<Vehicle, VehicleResponse>()
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : string.Empty))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type != null ? src.Type.Name : string.Empty));
    }
}