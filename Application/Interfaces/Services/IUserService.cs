using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Core.DTOs.Request.Customer;
using Core.Enums;

namespace Application.Interfaces.Services;
public interface IUserService
{
    Task<UserResponse?> GetUserByIdAsync(int id);
    Task<UserResponse> UpdateUserAsync(UserUpdateRequest request);
    Task<SearchResponse<UserResponse>> GetCustomersAsync(CustomerSearchRequest request);
    Task<UserResponse> UpdateUserStatusAsync(long userId, UserStatus status);
}