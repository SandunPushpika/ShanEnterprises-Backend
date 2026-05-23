using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Core.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
            return null;
        
        return MapToResponse(user);
    }
    public async Task<UserResponse> UpdateUserAsync(UserUpdateRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);

        if (user == null)
            throw new Exception("User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        var updatedUser = await _userRepository.UpdateUserAsync(user);

        return MapToResponse(updatedUser);
    }
    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}