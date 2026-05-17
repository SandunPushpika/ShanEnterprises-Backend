using Application.Interfaces.Services;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

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
}