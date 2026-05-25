using Application.Interfaces.Services;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : Controller
{
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> RegisterUser([FromBody] CreateUserRequest request)
    {
        await authService.RegisterUser(request);
        return new ApiResponse("Successfully registered user");
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> LoginUser([FromBody] LoginRequest request)
    {
        var result = await authService.LoginUser(request);
        return new ApiResponse("Successfully logged user", data: result);
    }

    [HttpGet("refresh-token")]
    public async Task<ActionResult<ApiResponse>> RefreshToken()
    {
        HttpContext.Request.Headers.TryGetValue("refresh-token", out var token);
        var refreshToken = token.FirstOrDefault();
        if(string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedAccessException();
        
        var response = await authService.RefreshToken(refreshToken);
        return new ApiResponse("Successfully refreshed user", data: response);
    }
}