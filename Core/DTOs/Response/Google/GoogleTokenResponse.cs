using System.Text.Json.Serialization;

namespace Core.DTOs.Response.Google;

public class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = default!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = default!;
}