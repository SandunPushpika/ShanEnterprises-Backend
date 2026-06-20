using Core.DTOs.Request.Auth;
using Core.DTOs.Response.Auth;
using Core.Entities;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<User> RegisterUser(CreateUserRequest request, bool socialMediaRequest = false);
    Task<LoginResponse> LoginUser(LoginRequest request);
    Task<LoginResponse> RefreshToken(string refreshToken);
    Task VerifyCode(string code);
    Task ResendVerificationCode(string email);
    string GetOAuthUrl();
    Task<LoginResponse> LoginViaSocialMedia(string code);
}