using System.Net.Http.Json;
using Core.DTOs.Response.Auth;
using Core.DTOs.Response.Google;
using Flurl;
using Core.Helpers;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class GoogleOAuthService : IOAuthService
{
    private const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
    private const string TokenUrl = "https://oauth2.googleapis.com/token";
    private const string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

    private readonly GoogleSettings _googleSettings;
    private readonly HttpClient _httpClient;

    public GoogleOAuthService(IOptions<AppSettings> appSettings, HttpClient httpClient)
    {
        _googleSettings = appSettings.Value.Google;
        _httpClient = httpClient;
    }

    public string GetAuthorizeUrl()
    {
        return AuthorizeUrl
            .SetQueryParams(new
            {
                scope = _googleSettings.Scopes,
                client_id = _googleSettings.ClientId,
                redirect_uri = _googleSettings.CallbackUrl,
                response_type = "code",
                access_type = "offline",
                prompt = "consent"
            });
    }

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        var request = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _googleSettings.ClientId,
            ["client_secret"] = _googleSettings.ClientSecret,
            ["redirect_uri"] = _googleSettings.CallbackUrl,
            ["grant_type"] = "authorization_code"
        };

        var response = await _httpClient.PostAsync(
            TokenUrl,
            new FormUrlEncodedContent(request)
        );

        response.EnsureSuccessStatusCode();

        var googleTokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>()
               ?? throw new Exception("Failed to deserialize token response");
        
        return new TokenResponse()
        {
            AccessToken = googleTokenResponse.AccessToken,
            RefreshToken = googleTokenResponse.RefreshToken,
            AccessTokenExpiration = googleTokenResponse.ExpiresIn,
        };
    }

    public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, UserInfoUrl);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var googleUser = await response.Content.ReadFromJsonAsync<GoogleUserInfoResponse>()
               ?? throw new Exception("Failed to deserialize user info");

        return new UserInfoResponse()
        {
            Email = googleUser.Email,
            FullName = googleUser.Name,
            ProfilePicture = googleUser.Picture,
        };
    }
}