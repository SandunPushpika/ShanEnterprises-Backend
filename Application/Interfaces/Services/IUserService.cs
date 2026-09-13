using Core.DTOs.Request.Auth;
using Core.DTOs.Request.User;
using Core.DTOs.Response;
using Core.DTOs.Request.Customer;
using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services;
public interface IUserService
{
    Task<UserResponse?> GetUserByIdAsync(int id);
    Task<UserResponse> UpdateUserAsync(UserUpdateRequest request);
    Task<UserResponse> GetCurrentUserProfileAsync();
    Task<UserResponse> UpdateCurrentUserProfileAsync(UserProfileUpdateRequest request);
    Task<UserResponse> UpdateProfileImageAsync(IFormFile file);
    Task<UserResponse> RemoveProfileImageAsync();
    Task<SearchResponse<UserResponse>> GetCustomersAsync(CustomerSearchRequest request);
    Task<UserResponse> UpdateUserStatusAsync(long userId, UserStatus status);
}