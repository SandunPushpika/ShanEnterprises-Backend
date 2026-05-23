using Application.Interfaces.Services;
using Core.DTOs.Request.Auth;
using Core.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShanEnterprises.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return new ApiResponse("User not found",success: false);

        return new ApiResponse("", data: user);
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdateUserDetails([FromBody] UserUpdateRequest request)
    {
        var updatedUser = await _userService.UpdateUserAsync(request);

        return new ApiResponse("User updated successfully", data: updatedUser);
    }
}