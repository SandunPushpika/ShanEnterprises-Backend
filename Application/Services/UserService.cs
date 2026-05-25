using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Core.Entities;
using Core.Enums;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IContextService _contextService;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IContextService contextService, IMapper mapper)
    {
        _userRepository = userRepository;
        _contextService = contextService;
        _mapper = mapper;
    }
    
    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var loggedUser = await _contextService.GetUser();
        if (loggedUser.Id != id && loggedUser.Role != UserRole.ADMIN)
            return null;
                
        var user = await _userRepository.GetUserByIdAsync(id);

        return user == null ? null : MapToResponse(user);
    }
    
    public async Task<UserResponse> UpdateUserAsync(UserUpdateRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);

        if (user == null)
            throw new Exception("User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.UpdatedAt = DateTime.UtcNow;
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);

        var updatedUser = await _userRepository.UpdateUserAsync(user);

        return MapToResponse(updatedUser);
    }
    
    private UserResponse MapToResponse(User user)
    {
        return _mapper.Map<UserResponse>(user);
    }
}