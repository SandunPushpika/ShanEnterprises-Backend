using AutoMapper;
using Core.DTOs.Request;
using Core.Entities;

namespace Application.Mappers;

public class VehicleProfile: Profile
{
    public VehicleProfile()
    {
        CreateMap<VehicleCreateRequest, Vehicle>();
        CreateMap<VehicleUpdateRequest, Vehicle>();
    }
}