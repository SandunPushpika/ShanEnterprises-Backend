using Core.DTOs.Request.Auth;
using Core.DTOs.Response.Auth;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task RegisterUser(CreateUserRequest request);
    Task<LoginResponse> LoginUser(LoginRequest request);
    Task<LoginResponse> RefreshToken(string refreshToken);
}