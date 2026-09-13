using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Request.User;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()));
        CreateMap<UserUpdateRequest, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()));
        CreateMap<UserProfileUpdateRequest, User>();
        CreateMap<User, UserResponse>();
    }
}