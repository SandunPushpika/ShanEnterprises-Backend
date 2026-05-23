using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response.Auth;
using Core.Entities;
using Core.Enums;
namespace Application.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.ACTIVE))
            .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())  
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<User, LoginResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore()) 
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());  
    }
}