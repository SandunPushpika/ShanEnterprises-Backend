using Core.Enums;

namespace Core.DTOs.Response.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserRole UserRole { get; set; }
}