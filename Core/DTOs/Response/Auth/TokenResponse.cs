namespace Core.DTOs.Response.Auth;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public long AccessTokenExpiration { get; set; }
    public long RefreshTokenExpiration { get; set; }
}