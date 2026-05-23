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
        //CreateUserRequest --> User
        CreateMap<CreateUserRequest, User>()
            
            //Auto-Mapped Properties 
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            
            // Properties that need special handling
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.ACTIVE))
            .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())  // Set in service
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            //User --> LoginResponse
        CreateMap<User, LoginResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.Ignore())  // Generated in service
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());  // Generated in service
    }
}