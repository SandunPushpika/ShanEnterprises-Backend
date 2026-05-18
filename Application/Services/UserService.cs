using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Core.DTOs.Request;
using Core.DTOs.Request.Auth;
using Core.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // view profile
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    // update user profile
    public async Task<User> UpdateUserAsync(UserUpdateRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);

        if (user == null)
            throw new Exception("User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        return await _userRepository.UpdateUserAsync(user);
    }
}