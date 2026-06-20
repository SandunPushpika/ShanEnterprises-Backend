using Core.DTOs.Response.Auth;

namespace Infrastructure.Interfaces;

public interface IOAuthService
{
    string GetAuthorizeUrl();
    Task<TokenResponse> ExchangeCodeForTokenAsync(string code);
    Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
}