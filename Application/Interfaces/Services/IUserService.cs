using Core.DTOs.Request.Auth;
using Core.DTOs.Response;

namespace Application.Interfaces.Services;
public interface IUserService
{
    Task<UserResponse?> GetUserByIdAsync(int id);
    Task<UserResponse> UpdateUserAsync(UserUpdateRequest request);
}