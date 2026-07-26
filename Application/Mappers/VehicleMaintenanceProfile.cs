using AutoMapper;
using Core.DTOs.Request.Maintenance;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Mappers;

public class VehicleMaintenanceProfile : Profile
{
    public VehicleMaintenanceProfile()
    {
        CreateMap<MaintenanceCreateRequest, VehicleMaintenance>();

        CreateMap<MaintenanceUpdateRequest, VehicleMaintenance>()
            .ForMember(dest => dest.VehicleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<VehicleMaintenance, MaintenanceReadResponse>()
            .ForMember(dest => dest.VehicleRegistrationNumber,
                opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.RegistrationNumber : null))
            .ForMember(dest => dest.VehicleName,
                opt => opt.MapFrom(src => src.Vehicle != null && src.Vehicle.Brand != null
                    ? $"{src.Vehicle.Brand.Name} {src.Vehicle.Model}"
                    : null))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));


    }
}