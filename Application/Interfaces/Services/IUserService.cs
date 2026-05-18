using Core.DTOs.Request;
using Core.DTOs.Request.Auth;
using Core.Entities;

namespace Application.Interfaces.Services;

public interface IUserService
{
    //view user profile
    Task<User?> GetUserByIdAsync(int id);

    //update user profile
    Task<User> UpdateUserAsync(UserUpdateRequest request);
}