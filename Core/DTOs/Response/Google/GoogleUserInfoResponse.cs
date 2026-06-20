using System.Text.Json.Serialization;

namespace Core.DTOs.Response.Google;

public class GoogleUserInfoResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("verified_email")]
    public bool VerifiedEmail { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; } = default!;

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; } = default!;

    [JsonPropertyName("picture")]
    public string Picture { get; set; } = default!;
}